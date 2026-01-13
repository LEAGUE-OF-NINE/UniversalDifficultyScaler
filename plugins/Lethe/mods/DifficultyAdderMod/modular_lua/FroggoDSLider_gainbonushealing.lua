function gain_bonus_healing_on_hit()
    local id = instid("MainTarget")
    local target = "inst" .. id
    local dmg = getldata("Self", "Bonus Flat Healing On Hit")
    healhp(target, dmg)
end

function gain_bonus_healing_on_combat_start()
    local dmg = getldata("Self", "Bonus Flat Healing On Combat Start")
    healhp("Self", dmg)
end
