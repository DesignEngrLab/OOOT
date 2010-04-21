function [C,Ceq]=genocop11_con(x)
%function:
%(1)input:the input variable ,x is one point.it can be a column vector or a row
%vector.
%*****note that It can not evaluate the function at multiple points at
%once,but you can call the fuction multiple times.
%(2)output:f is a scalar,not a vector.
%
%reference:
%note that you can get the formulation of genocop11_con from some
%aritcles,such as
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 11 
%
%algorithm:
%below code is  edited according to 
%(1)Michalewicz ,Zbigniew 'Genetic Algorithms+ Data Structures= Evolution Programs' third edition
%1996,Appendix C case 11 
%
%solution:
% over=10;
% n=6;lb=[0*ones(n,1)];ub=[over;over;over;1;1;2];
%The global minimum is  [ 0    6   0   1   1 0]';
%and f=-11;



%Copyright:
% programmers:oiltowater.
% It comply with the GPL2.0
% Copyright 2006  oiltowater 






%change the row vector to a column vector.
if size(x,1)==1,
    x=x';
end

if size(x,1)~=6,
    error('the length of x must be 6!');
end


x1=x(1,1);y1=x(2,1);y2=x(3,1);y3=x(4,1);
y4=x(5,1);y5=x(6,1);





Ceq=0;
A=[1 2 8 1 3 5;
    -8 -4 -2 2 4 -1;
    2 0.5 0.2 -3 -1 -4;
    0.2 2 0.1 -4 2 2;
    -0.1 -0.5 2 5 -5 3];

C(1,1)=A(1,:)*x-16;
C(2,1)=A(2,:)*x+1;
C(3,1)=A(3,:)*x-24;
C(4,1)=A(4,:)*x-12;
C(5,1)=A(5,:)*x-3;



