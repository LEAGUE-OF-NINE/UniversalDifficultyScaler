function gain_stagger_on_hit()
    local id = instid("Self")
    local target = "inst" .. id
    local dmg = getldata(target, "Change Stagger On Self On Hit")
    breakdmg(target, dmg, 1)
end

function gain_stagger_when_hit()
    local id = instid("MainTarget")
    local target = "inst" .. id
    local dmg = getldata(target, "Change Stagger On Self When Hit")
    breakdmg(target, dmg, 1)
end
