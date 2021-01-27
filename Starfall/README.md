# Starfall Samples
If you want to spawn an example, download the auto-generated Lua file and save it in your `garrysmod/data/starfall` folder.  
Note, **do not** open the generated Lua file with in-game editor because it will freeze GMod.  
Instead create a separate file and then include the desired generated Lua, for example (for [Fireplace](#Fireplace)):  
```lua
--@client
--@include Fireplace.min.lua
require("Fireplace.min.lua")
```
By doing so, you won't freeze up your GMod.  

## [Fireplace](https://github.com/dnGLua/Samples/blob/main/Starfall/Fireplace/Fireplace.cs)
Ported from the [example source code](https://github.com/thegrb93/StarfallEx/blob/master/lua/starfall/examples/fireplace.lua).  
Auto-generated (also minified) Starfall file is [available here](https://github.com/dnGLua/Samples/raw/main/Starfall/Fireplace/Fireplace.min.lua).  
