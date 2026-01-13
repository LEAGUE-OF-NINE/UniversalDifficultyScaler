function deal_bonus_damage()
    local id = instid("MainTarget")
    local target = "inst" .. id
    local dmg = getldata("Self", "Bonus Damage On Hit")
    bonusdmg(target, dmg, -1, -1)
end

deal_bonus_damage()
