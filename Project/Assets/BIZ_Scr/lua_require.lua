----------------------------------------------------------------
-- File			: Assets\BIZ_Scr\lua_require.lua
-- Author		: www.loywong.com
-- COPYRIGHT	: (C)
-- Date			: 2019/07/29
-- Description	: Lua全局脚本 包含以下部分
                -- 1. 底层框架层(Core)
                -- 2. 业务数据层
                -- 3. 业务逻辑代码层
-- Version		: 1.0
-- Maintain		: //[date] desc
----------------------------------------------------------------

-- Core - Util
require("Core/define")
require("Core/extensions")
require("Core/functions")

require("Core/log")
require("logsetting")

-- Core - Module
require("Core/inputmanager")

-- Data
require("DT/define")

-- FN
require("FN/fn_account")
require("FN/fn_login")

-- Controller
require("gamecontroller")