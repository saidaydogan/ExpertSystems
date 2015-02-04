:- [cikarim].

program :-
    open('X.txt',write, Stream),
    write('Mekan girin'),nl,
    read(X),
    write('Eylem girin'),nl,
    read(Y),
    cumle(_,_,_,X,Y,Stream),
    close(Stream).
