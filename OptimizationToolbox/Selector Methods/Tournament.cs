using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
 public   class Tournament : abstractSelector
    {
     public Tournament(optimize direction) : base(direction)
     {
     }

     public override SortedList<double, double[]> selectCandidates(SortedList<double, double[]> sortedList, double control = double.NaN)
     {
         throw new NotImplementedException();
     }
    }
}



//function parents = selectiontournament(expectation,nParents,options,tournamentSize)
//%SELECTIONTOURNAMENT Each parent is the best of a random set.
//%   PARENTS = SELECTIONTOURNAMENT(EXPECTATION,NPARENTS,OPTIONS,TOURNAMENTSIZE)
//%   chooses the PARENTS by selecting the best TOURNAMENTSIZE players out of 
//%   NPARENTS with EXPECTATION and then choosing the best individual 
//%   out of that set.
//%
//%   Example:
//%   Create an options structure using SELECTIONTOURNAMENT as the selection
//%   function and use the default TOURNAMENTSIZE of 4
//%     options = gaoptimset('SelectionFcn',@selectiontournament); 
//%
//%   Create an options structure using SELECTIONTOURNAMENT as the
//%   selection function and specify TOURNAMENTSIZE to be 3.
//%
//%     tournamentSize = 3;
//%     options = gaoptimset('SelectionFcn', ...
//%               {@selectiontournament, tournamentSize});

//%   Copyright 2003-2007 The MathWorks, Inc.
//%   $Revision: 1.6.4.2 $  $Date: 2007/05/23 18:50:24 $

//% How many players in each tournament?
//if(nargin < 4)
//    tournamentSize = 4;
//end

//% Choose the players
//playerlist = ceil(size(expectation,1) * rand(nParents,tournamentSize));
//% Play tournament
//parents = tournament(playerlist,expectation);

//function champions = tournament(playerlist,expectation)
//%tournament between players based on their expectation

//playerSize = size(playerlist,1);
//champions = zeros(1,playerSize);
//% For each set of players
//for j = 1:playerSize
//    players = playerlist(j,:);
//    % For each tournament
//    winner = players(1); % Assume that the first player is the winner
//    for j = 2:length(players) % Winner plays against each other consecutively
//        score1 = expectation(winner,:);
//        score2 = expectation(players(j),:);
//        if score2(1) < score1(1) 
//            winner = players(j);
//        elseif score2(1) == score1(1)
//            try % socre(2) may not be present for single objective problems
//                if score2(2) > score1(2)
//                    winner = players(j);
//                end
//            catch
//            end
//        end
//    end
//    champions(j) = winner;
//end

 