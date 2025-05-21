
function getPassiveDescription(passiveId) {
	if (passiveId == 0) { return "No Passive"; }
	if (passiveId == 1) { return "Heavy Smash: 10% chance to gain +3 attack on any attack. 20% for Warriors"; }
	if (passiveId == 2) { return "Spiked Bulwark: Returns 1 damage to the enemy when you are attacked. 2 for Warriors"; }
	if (passiveId == 3) { return "Arcane Recovery: 20% chance to reduce the Cooldown of your active ability by an extra turn on an attack. 40% for Mages"; }
	if (passiveId == 4) { return "Mana Burn: 10% chance to deal 2 true damage, increased by your remaining Ability Cooldown, when you use an attack. 20% for Mages."; }
	if (passiveId == 5) { return "Quick Shot: 5% chance to attack twice on any attack. 10% for Archers"; }
	if (passiveId == 6) { return "Poison Weapons: 10% chance to permanently reduce the enemy's defense by 1 on any attack. 20% for Archers."; }
	return "Unknown Passive";
}

function getActiveDescription(_class) {
	_class=_class.toLowerCase()
	if (_class == "warrior") { return "Reckless Strike: Perform an attack at double strength."; }
	if (_class == "mage") { return "Magic Barrier: Gain +6 defense against the next attack."; }
	if (_class == "archer") { return "Twin Shot: Perform two attacks."; }
	return "Unknown Active"
}