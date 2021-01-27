// Starfall Fireplace reimagined in C# (for use with dnGLua.Compiler)

using System.Collections.Generic;
using static _G;

namespace Fireplace
{
    public static class Program
    {
        public static void Main()
        {
            Realm realm = CLIENT ? new ClientsideRealm() : new ServersideRealm();
            realm.Main(); // Strategically, run clientside or serverside depending on the realm respectively.
        }
    }

    public sealed class ClientsideRealm : Clientside
    {
        const string RtName = "fireplace";
        List<Particle> _particles = new List<Particle>();
        double _t, _t2;
        Vector _relativeVelocity;

        public override void Main()
        {
            render.CreateRenderTarget(RtName);

            // Testing out custom event/hooking API
            var renderHook = new RenderScreen(OurRenderRoutine);
            renderHook.Activate();
        }

        void OurRenderRoutine()
        {
            // This is where the 'render' hook lives :)

            render.SetRenderTargetTexture(RtName);
            render.DrawTexturedRect(0, 0, 512, 512);

            render.SelectRenderTarget(RtName);

            if (SysTime() > _t)
            {
                for (int i = 1, to = math.random(4, 10); i <= to; ++i)
                {
                    var np = new Particle
                    {
                        X = math.random(-512, 512),
                        Y = math.random(-64, 32),
                        Scale = math.random(40, 80),
                        VelocityX = math.random() * 5 - 2,
                        VelocityY = -math.random() * 3,
                        // Testing out reinterpret cast magic...  :D
                        Color = reinterpret_cast<Color, ColorF>(Color(reinterpret_cast<int, byte>(math.random(200, 230)), reinterpret_cast<int, byte>(math.random(100, 130)), 0, reinterpret_cast<int, byte>(math.random(120, 255))))
                    };
                    _particles.Add(np);
                }

                _t = SysTime() + (1.0 / 20);
            }

            if (SysTime() > _t2)
            {
                _relativeVelocity = Chip.WorldToLocal(Chip.Pos + Chip.Velocity);

                render.Clear(Color(5, 5, 16));

                // Testing out custom looping API :)
                _particles.each((k, part) =>
                {
                    if (part.Color.a <= 0) { table.remove<Particle>(_particles, k); return; }

                    part.VelocityX -= _relativeVelocity.y * (1.0 / 5000) * (80 / part.Scale);
                    part.VelocityY -= _relativeVelocity.x * (1.0 / 2000) * (80 / part.Scale);

                    part.Think();
                });

                _t2 = SysTime() + (1.0 / 120);
            }

            render.SelectRenderTarget(null);
        }
    }

    public sealed class ServersideRealm : Serverside { }

    public class Particle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Scale { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public ColorF Color { get; set; } // Our special color struct with floating-point fields because of the fireplace math (see Think)...

        public virtual void Think()
        {
            Draw();

            X += VelocityX;
            Y += VelocityY;

            VelocityY -= math.Rand(0, 0.04);

            Color.a -= math.Rand(1.5, 3);

            var c = Color.g - math.Rand(0.65, 0.95);
            Color.g = (c > 0) ? c : 0;

            c = Color.r + math.Rand(0.4, 0.6);
            Color.r = (c < 255) ? c : 255;

            Scale -= math.Rand(0.2, 0.3);
        }

        public virtual void Draw()
        {
            // Cast our custom ColorF struct into Color struct which `render.SetColor` expects
            render.Color = reinterpret_cast<ColorF, Color>(Color); //Color((byte)Color.r, (byte)Color.g, (byte)Color.b, (byte)Color.a);
            // More reinterpret_cast magic :P
            render.DrawRect(reinterpret_cast<double, int>(X + 512), reinterpret_cast<double, int>(Y + 780), reinterpret_cast<double, int>(Scale), reinterpret_cast<double, int>(Scale));
        }
    }

    #region A nice example, we can define missing API directly, right here and now, without waiting for API (nuget) update

    /// @CSharpLua.Ignore
    public static class Extensions
    {
        /// @CSharpLua.Template = {0}:worldToLocal({1})
        public static extern Vector WorldToLocal(this BaseEntity @this, Vector offset);
    }

    /// @CSharpLua.Ignore
    public sealed partial class ColorF
    {
        public double r, g, b, a;

        private extern ColorF();

        /// @CSharpLua.Template = _G.Color({0}, {1}, {2})
        public extern ColorF(double r, double g, double b);

        /// @CSharpLua.Template = _G.Color({0}, {1}, {2}, {3})
        public extern ColorF(double r, double g, double b, double a /*= 0xFF*/);
    }

    #endregion
}
