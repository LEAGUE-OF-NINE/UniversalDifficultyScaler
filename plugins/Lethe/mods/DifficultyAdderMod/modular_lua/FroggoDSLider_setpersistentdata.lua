function load_in_specific_data(path, buff)
    -- log("We have like. triggered. at all?")
    local stringFile = readfile(path)
    local table = jsontolua(stringFile)
    local target = "Ally99" .. buff
    log("we are currently reading in a file from" .. path)
    log("We got da pizza: " .. target)
    for key, value in next, table
    do
        log("I am a file entry, my key is: " .. key .. " and my value is" .. value)
    end

    local targets = selecttargets(target)
    for key, unit in next, targets
    do
        setldata(unit, "Bonus Damage On Hit", table["Bonus Damage On Hit"])
        setldata(unit, "Bonus Flat Healing On Hit", table["Bonus Flat Healing On Hit"])
        setldata(unit, "Bonus Flat Healing On Combat Start", table["Bonus Flat Healing On Combat Start"])
        setldata(unit, "Change Stagger On Self On Hit", table["Change Stagger On Self On Hit"])
        setldata(unit, "Change Stagger On Self When Hit", table["Change Stagger On Self When Hit"])
    end

    -- log("For our unit, the current Bonus Damage On Hit has been set to: " .. getldata("Self", "Bonus Damage On Hit"))

    -- for key, value in next, table
    -- do
    --     log("I am a file entry, my key is: " .. key .. " and my value is" .. value)
    -- end
end
