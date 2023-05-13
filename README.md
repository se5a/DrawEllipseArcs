# DrawEllipseArcs
Goal of this was to explore different ways of outputting an array of points to draw ellipse arcs. 

I wanted to be able to draw an ellipse with a limited number of points that would still look good at the apsies. 
This paper describes a way to do this for a full ellipse: https://academic.oup.com/comjnl/article/14/1/81/356378
But trying to modify that to draw an arc was beyond my math skill. 

one other way I explored was to draw the points from the focal point and switch focal points half way, I did not get far with that. 

Third attempt was a bit more succesful where I realised if I drew a circle (or arc) with 0 eccentricity, 
then used a matrix scale transform on it it would give me simular results to the paper. 

Stuff.cs has a bunch of working, non working and half working functions of different experiments.
I'm using a hand rolled matrix class and and hand rolled vector class to give me vectors with doubles 
(this was due to the origional requirement needing to work in solar system meter scales). 

uses SDL2 to draw the points and dear imgui.net for the interface controls,  
this part of the code is a little messy as I was attempting to get something together to display stuff quickly so it's copypastad from another project without a lot of care. 
