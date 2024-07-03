* Create soft bounds:
	* decellarate the player as they approach the borders
	* Prevents hard stop
	* prevents stuck acceleration

* code NPCs to follow shooting patterns
	* Patterns get more complex in later levels
	* NPCs slowly approach the player
		* their x velocity is existant but very small
		* the player can do things to back them up
	* NPCs have motions that they take throughout the level
	* NPCs have shooting patterns
	* Create classes that define the shooting patterns and classes that NPCs can take from at random
		* These classes should have a means of making them more difficult
	* Randomly select when an NPC will dodge a trap
		* Should be gagued with a number (first level has zero avoidance factor)
	* Maybe when an NPC leaves the screen it recycles back to the beginning

* Gadgets
	* gadgets can be collected throughout the level
	* combine gadgets to make new gadgets
	* can have a pack, but only 2 gadgets at a time (a and b buttons)
	* Gadgets (collaborated with Dylan Cassidy):
		* Smoke bomb - randomly makes enemy fire go in other directions
		* Oil - slows down the enemy
			* Make emeny head the same color when hit
		* Cannon? - makes the enemy veer off into a direction
		* hologram - draws enemy fire
		* Reactor - absorbs enemy fire to then dish out energy blast (diameter dependent on amount of fire absorbed)
		* Shield - no explanation needed
		* teleporter - jumps user to a spot where an enemy isn't
* Dungeon level
	* we can make the dungeon level as just an in-between levels
	* this would be where you configure gadgets
	* Gadgets made for a certain situation determine how many NPCs you experience
		* gadgets can come with decisions

* Remaining Sprites Needed:
	* Oil
	* cannon
	* sheild
	* holorgram

Sunday TODO:
* Draw above sprites
* Implement gadget interactions
* implement UI

Post-Jam Features:
* Implement gadget cancel?

Up Next:
* add game over
* add means to restart
* fine tune config data
	* increasing difficulty?
	* FIX scoring
* SFX
* Entering leaderboard
* Add canMove bool to menus

Bugs:
* Enemies spawinging outside of the boundaries
* Shield will sometimes not hold for the proper amount of time
* bomb not reaching enemies when unplugged
* text moves to slow when unplugged
* NPC aiming at hologram is off (maybe move fire up and down between a 32 px range?)
* See what's going on with the oil ball and freeing when the enemy is already oiled
* Maybe change the oil animation to look more drippy?
