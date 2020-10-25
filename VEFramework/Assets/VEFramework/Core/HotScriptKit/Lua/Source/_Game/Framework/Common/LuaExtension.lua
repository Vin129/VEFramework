-- table改造
function table.refit(tab)
    local mtab = {}
    mtab.__add = function (a,b)
        local c = {}
        for k,v in pairs(a) do
            c[k] = a[k] + b[k]
        end
        setmetatable(c,mtab)
        return c
    end
    setmetatable(tab,mtab)
    return tab
end

function table.readonlyTab(tab)
    if type(tab) ~= "table" then
        return tab
    end
    for k,v in pairs(tab) do
        if type(tab[k]) == "table" then
            tab[k] = table.readonlyTab(tab[k])
        end
    end
    local mt = getmetatable(tab)
    if mt then
        mt.__newindex = function (t,k,v)
                        logError("readonly table!")
                        UIUtil:showTips(nil,{type = TipPopType.HaveBtn},"只读table不可修改("..tostring(k)..")，请改代码")
                    end
    end
    return tab
end

function table.erase(t,val)
	table.eraseBy(t,function(v) return v == val end)
end
-- 根据条件删除kvTab中数据
function table.eraseBy(t,cb)
	for k,v in pairs(t) do
		if cb(v) then
			t[k] = nil
		end
	end
end

-- KV变数组
function table.toArray(tab)
    local array = {}
    for k,v in pairs(tab) do
        table.insert(array,v)
    end 
    return array
end

-- 将 2000-01-01 00:00:00 这种形式的字符串转换成os.time()
-- 之后一个个弃用掉此功能，转换有上限不适合被使用
-- 默认为北京时间+8
function totime(timeStr,isSecond)
    if isSecond and type(timeStr) ~= "string" then
        return timeStr
    end
    if type(timeStr) ~= "string" then
        return timeStr/1000
    end

    local timeTab = {
        y = string.sub(timeStr,1,4),
        m = string.sub(timeStr,6,7),
        d = string.sub(timeStr,9,10),
        h = string.sub(timeStr,12,13),
        mm = string.sub(timeStr,15,16),
        ss = string.sub(timeStr,18,19),
    }
    local dt = os.time {
        year = timeTab.y,
        month = timeTab.m,
        day = timeTab.d,
        hour = timeTab.h,
        min = timeTab.mm,
        sec = timeTab.ss
    }
    dt = dt - 16 * 3600 + os.time{year = 1970, month = 1, day = 2, hour = 0}
    return dt
end

-- 三目表达式
function math.ab(condition,a,b)
    return (condition and {a} or {b})[1]
end

--转二进制tab
function math.toBinaryTab(num)
    local binaryTab = {}
    local index = 1
    local d,p = math.modf(num/2)
    while d>=1  do
        if p ~= 0 then
            binaryTab[index] = 1
        else
            binaryTab[index] = 0
        end
        index = index + 1
        d,p = math.modf(d/2)
    end
    if p ~= 0 then
        binaryTab[index] = 1
    else
        binaryTab[index] = 0
    end
    return binaryTab
end

-- 数字转中文 0~99
function string.numtoChiness(num)
    local chinessNum = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "百", "千", "万", "亿"}
    num = tonumber(num)
    local numStr = tostring(num)
    local length = #numStr
    if length == 1 then
        return chinessNum[num]
    elseif length == 2 then
        if num == 10 then
           return chinessNum[num] 
        elseif num > 10 and num < 20 then
            return "十"..chinessNum[tonumber(string.sub(numStr,2,2))]
        else
            if num%10 == 0 then
                return chinessNum[tonumber(string.sub(numStr,1,1))].."十"
            else
                return chinessNum[tonumber(string.sub(numStr,1,1))].."十"..chinessNum[tonumber(string.sub(numStr,2,2))]
            end
        end
    end
end

--是否包含
function table.contains(tab,val)
    for k,v in pairs(tab) do
        if v == val then
            return true
        end
    end
    return false
end

function second2DHMS(second)
    local d = Mathf.floor(second / (24 * 3600))
    local h = Mathf.floor(second % (24 * 3600) / 3600)
    local m = Mathf.floor(second % 3600 / 60)
    local s = Mathf.floor(second % 60)

    local res = ""
    if d > 0 then
        res = res..d.."天"
    end
    if h > 0 then
        res = res..h.."时"
    else
        if d > 0 then
            res = res..h.."时"
        end
    end
    if m > 0 then
        res = res..m.."分"
    else
        if d > 0 or h > 0 then
            res = res..m.."分"
        end
    end

    if d <= 0 then
        res = res..s.."秒"
    end
    
    return res
end

--秒转换成时间戳数据
function second2Date(second)
    local date = {}
    date.day = Mathf.floor(second / (24 * 3600))
    date.hour = Mathf.floor(second % (24 * 3600) / 3600)
    date.min = Mathf.floor(second % 3600 / 60)
    date.sec = Mathf.floor(second % 60)
    return date
end

--秒转换成时间戳数据
function second2HMS(second)
    local h = Mathf.floor(second / 3600)
    local m = Mathf.floor((second - 3600*h) / 60)
    local s = second - 3600*h - m*60
    
    return string.format("%02d:%02d:%02d",h,m,s)
end