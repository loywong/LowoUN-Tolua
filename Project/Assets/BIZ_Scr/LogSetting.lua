----------------------------------------------------------------
-- File			: Assets\BIZ_Scr\LogSetting.lua
-- Author		: www.loywong.com
-- Company		: ShangHai ShiWan Co.,Ltd
-- COPYRIGHT	: (C)
-- Date			: 2019/08/20
-- Description	:
                -- 控制log的开启美闭， 以及定制标签
                -- 标签分为3类
                -- 1. 框架层CORE
                --      1-1功能
                --      1-2模块
                --      1-3流程
                -- 2. 业务层BIZ(所有C#都要翻译成Lua)
                -- 3. 个⼈⾃由定制层PERS(personal)

                -- 颜⾊设置规范!!!
                -- Blue	        Model	    蓝⾊(数据)
                -- Gray	        View	    灰⾊(极图)
                -- Green        Controller  绿⾊(流程)

                -- orange	相当于  Debug.warn
                -- Red      相当于  Debug.Error
                -- Tracfe	相当于  Debug.Log 其他⼀般log
-- Version		: 1.0
-- Maintain		: //[date] desc
----------------------------------------------------------------

if not IsEditor() then
    lualog.isDefaultStack = false
end

-- 控制c#层的Log⼀但完全由Lua实现，则C#层只有框架⽬志//////////////////////////////
--------------------------------------------------------------------------------------------------------

-- // // 1 CORE -----------------------------------------
-- // // 1-1 功能
-- // Log.OpenTag("util") ;	                        //Trace⽩⾊

--////1-2模块
Log.OpenTag("net")
Log.OpenTag("socket")
if IsEditor() then
    Log.OpenTag("tick")
end

Log.OpenTag("audio")
Log.OpenTag("asset")
Log.OpenTag("ui")

-- 1-3 流程
Log.OpenTag("scene")                        --//Green绿⾊
Log.OpenTag("flow")                         --//Green绿⾊ workflow
Log.OpenTag("login")                        --//Green绿⾊
Log.OpenTag("sdk")
Log.OpenTag("test")

-- // // 3 PERS--------------------------------------
Log.OpenTag("loywong")