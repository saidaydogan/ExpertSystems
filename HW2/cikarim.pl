:- multifile cumle/5.

:- [fiiller].
:- [isimler].


vardir(Mekan,Stream):-iliski(Fiil,'nerede yapýlýr?',Mekan),vardir2(Fiil,Stream).
vardir2(Fiil,Stream):-iliski(Fiil,'kim/ne yapar?',A),write(Stream,Fiil),write(Stream,' yapan '),write(Stream,'Özne-->'),write(Stream,A),write(Stream,'\n').

vardir3(Eylem,Stream):-iliski(Eylem,'nasýl yapýlýr?',Zarf),write(Stream,'Durumu-->'),write(Stream,Zarf),write(Stream,'\n'),write(Stream,Eylem),write(Stream,' '),write(Stream,Zarf),write(Stream,' yapýlýr'),write(Stream,'\n'),vardir4(Eylem,Stream).
vardir4(Eylem,Stream):-iliski(Eylem,'niçin yapýlýr?',Eylem2),vardir5(Eylem2,Stream).

vardir5(Eylem,Stream):-iliski(Eylem,'yapýnca ne olur?',Zarf),write(Stream,'Durumu-->'),write(Stream,Zarf),write(Stream,'\n'),write(Stream,Eylem),write(Stream,' '),write(Stream,Zarf),write(Stream,' yapýlýr'),write(Stream,'\n').

cumle(_,_,_,Mekan,Eylem,Stream):-vardir(Mekan,Stream),vardir3(Eylem,Stream).

