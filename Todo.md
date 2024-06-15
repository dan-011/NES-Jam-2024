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

* Gadgets
    * gadgets can be collected throughout the level
    * combine gadgets to make new gadgets
    * can have a pack, but only 2 gadgets at a time (a and b buttons)
    * Gadgets (collaborated with Dylan Cassidy):
        * Smoke bomb - randomly makes enemy fire go in other directions
        * Oil - slows down the enemy
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

Up Next: gadgets and health