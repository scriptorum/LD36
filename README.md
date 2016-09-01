# LD36
Ludum Dare 36 (48H compo)

## The Trade Network
Build a trade network connecting all the villages.

### How to Play

- Click next to any village to build your first road.
- This connects the village to your trade network, increasing your income.
- To build more roads, click next to a road or a connected village.
- Villages come in three sizes. Connecting them gives you 1-3 income.
- Building roads over plains, forest, mountains or water costs 1-4 coins respectively.
- When you're out of money, press SPACE to advance to next year.
- When the year advances, you gain income from your villages, but lose coins to taxes.
- The tax rate then goes up by 2 coins.
- Connect all the villages to your road network before you are taxed into bankruptcy.

## TODO

These are things I'd like to get in before the compo ends. Naturally, some won't make it. 
It's loosely ordered by priority.
- [ ] Levels
- [ ] Add animation curve to message popup
- [ ] Separate bank/income/tax for better coinbank anim?
- [ ] Colorize text when income < tax
- [ ] Put "R/ESC" tip on message after showing victory and hold it there
- [ ] Tutorial
- [ ] SFX: Add crash to start
- [ ] Rerecord musical queues with longer tail
- [ ] Improve coin art
- [ ] Music: Play
- [ ] Additional mechanics: Resources? Will require additional level design and change to Tutorial.
- [ ] Folks walking down roads
- [ ] Better road art
- [ ] SFX: different sounds for different sized villages or raising pitch for each subsequent
- [ ] ~~FX: Animate coins in coinbank~~ [scrapped]
- [X] BUGFIX: Slight improvement to audio timing glitch when playing theme. #WebGLProblems
- [X] BUGFIX: Theme volume levels were mixed incorrectly, sounds better now.
- [X] BUGFIX: Less chance of game getting cropped or looking funny in full screen.
- [X] BUGFIX: Made some of the text elements sharper, not sure it's better. :)
- [X] Help button/drop down interface
- [X] FX: Dust when building road
- [X] FX: Village connected
- [X] SFX: build road
- [X] SFX: can't build road
- [X] SFX: gain village
- [X] SFX: lose game
- [X] SFX: button click
- [X] Music: Main menu
- [X] SFX: year end
- [X] SFX: win game
- [X] SFX: start game
- [X] Main menu buttons
- [X] Main menu background
- [X] SFX: Improve village clang - it's too brusque
- [X] SFX: road building, make it softer
- [X] Random level mode
- [X] More informative help screen since you're out of time to do a tute

## Post Compo

### Extra levels
Here are some decent solvable levels I've found. In the post-compo version copy this code, 
open any level and then press P to paste it. Likewise you can get the current level's code by pressing C. 
On standalone versions this works by interacting with the clipboard, but on WebGL (pthbth) 
you are instead presented with a javascript dialog box. Anyhoot, this code is the random seed 
used to generate the level. 

R1833308256
R1197157162
R-1227625050
R-1463715304
R611065350
R-717291648

### Editor
I've exposed the basic editor. Press E to toggle edit mode. Point to any hex and press a 
number key, or hold down the key and drag to paint. 0-3 are terrain features, and 4-6 are
the villages. Once edited you can copy the code with the C key, which is longer than a 
random level key but "pastes" just the same. (C#####... instead of R####.)

