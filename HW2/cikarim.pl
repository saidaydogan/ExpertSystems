:- multifile cumle/5.

:- [fiiller].
:- [isimler].


vardir(Mekan,Stream):-iliski(Fiil,'nerede yap�l�r?',Mekan),vardir2(Fiil,Stream).
vardir2(Fiil,Stream):-iliski(Fiil,'kim/ne yapar?',A),write(Stream,Fiil),write(Stream,' yapan '),write(Stream,'�zne-->'),write(Stream,A),write(Stream,'\n').

vardir3(Eylem,Stream):-iliski(Eylem,'nas�l yap�l�r?',Zarf),write(Stream,'Durumu-->'),write(Stream,Zarf),write(Stream,'\n'),write(Stream,Eylem),write(Stream,' '),write(Stream,Zarf),write(Stream,' yap�l�r'),write(Stream,'\n'),vardir4(Eylem,Stream).
vardir4(Eylem,Stream):-iliski(Eylem,'ni�in yap�l�r?',Eylem2),vardir5(Eylem2,Stream).

vardir5(Eylem,Stream):-iliski(Eylem,'yap�nca ne olur?',Zarf),write(Stream,'Durumu-->'),write(Stream,Zarf),write(Stream,'\n'),write(Stream,Eylem),write(Stream,' '),write(Stream,Zarf),write(Stream,' yap�l�r'),write(Stream,'\n').

cumle(_,_,_,Mekan,Eylem,Stream):-vardir(Mekan,Stream),vardir3(Eylem,Stream).

