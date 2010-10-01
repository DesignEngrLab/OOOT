DECLARE SUB GETFUN (N%, X#(), F#)
DECLARE SUB QUADFIT (N%, X0#(), X#(), D#(), F1#, XSTAR#, FSTAR#, STP#)
DECLARE SUB THREEPOINT (N%, X1#, X2#, X3#, F1#, F2#, F3#, STP#, X0#(), X#(), D#())
DECLARE SUB GETFUN1 (N%, X0#(), X#(), D#(), AL#, F#)
DEFINT I-N
DEFDBL A-H, O-Z
        '*****    POWELL'S ZERO ORDER METHOD *****
        '*     A.D.BELEGUNDU & T.R.CHANDRUPATLA   *
        '*****************************************
        COMMON SHARED NFV
        CLS : COLOR 1, 3: LOCATE 1, 1
        PRINT "POWELL'S ZERO ORDER METHOD"; SPACE$(25);
        PRINT "(C)  Belegundu & Chandrupatla": COLOR 7, 0
        VIEW PRINT 2 TO 25
'#var., Alpha, Beta, Initial Step, Step Reduction Parameter
        READ N, STP, EPS
        DATA 2, 1, .0001
        DIM X(N), XT(N), V(N, N), D(N), U(N)
'Read Start point coordinates
        FOR J = 1 TO N
           READ X(J)
           XT(J) = X(J): U(J) = 0
        NEXT J
        DATA -1.2,1
        'DATA .5,1,.5
        'DATA 1,.5
        NFV = 0
        CALL GETFUN(N, XT(), FT)
        PRINT "Starting Value of the function "
        PRINT FT
        PRINT "Stage#  Function value "
        FOLD = FT
        ISTG = 0
DO
'----- Loop for Each Stage -----
     ISTG = ISTG + 1
    '----- Reset after every stage
     FOR I = 1 TO N
        FOR J = 1 TO N
           V(I, J) = 0
        NEXT J
        V(I, I) = 1
     NEXT I
   '----- Develop N conjugate directions
     FOR JJ = 1 TO N
        IDIR = JJ - 1
        IF IDIR < 1 THEN IDIR = N
   '----- Search N + 1 Directions
        FOR II = 1 TO N + 1
           '----- Try along Direction IDIR / Column IDIR of V
           FOR J = 1 TO N: D(J) = V(J, IDIR): NEXT J
           CALL QUADFIT(N, XT(), X(), D(), FT, TSTAR, FSTAR, STP)
           FOR J = 1 TO N
              TD = TSTAR * D(J)
              XT(J) = XT(J) + TD
              IF II > 1 THEN U(J) = U(J) + TD
           NEXT J
           IDIR = IDIR + 1
           IF IDIR > N THEN IDIR = 1
           FT = FSTAR
        NEXT II
     FOLD = FT
     '--- U() into V()
        FOR J = 1 TO N: V(J, JJ) = U(J): U(J) = 0: NEXT J
     NEXT JJ
     '----- Check along the final conjugate direction
     FOR J = 1 TO N: D(J) = V(J, N): NEXT J
     CALL QUADFIT(N, XT(), X(), D(), FT, TSTAR, FSTAR, STP)
     FOR J = 1 TO N
        TD = TSTAR * D(J)
        XT(J) = XT(J) + TD
     NEXT J
     FT = FSTAR
     '----- Spacer Step -- One iteration of Cyclic coordinate
     FOR II = 1 TO N
        '----- Try along Direction II
        FOR J = 1 TO N: D(J) = 0: NEXT J
        D(II) = 1
        CALL QUADFIT(N, XT(), X(), D(), FT, TSTAR, FSTAR, STP)
        XT(II) = XT(II) + TSTAR
        FT = FSTAR
     NEXT II
     '----- Check convergence
     PRINT ISTG; TAB(8); FT
     IF ABS(FT - FOLD) < EPS + .0001 * ABS(FT) THEN EXIT DO
     FOLD = FT
LOOP
'=====  CONVERGED  =====
        PRINT "Function value = ";
        PRINT USING "##.#####^^^^ "; FT
        FOR J = 1 TO N
           PRINT "   X("; J; ") = ";
           PRINT USING "##.#####^^^^ "; XT(J)
        NEXT J
        PRINT "Number of Stages = "; ISTG
        PRINT "Number of Function Evaluations = "; NFV
        END

        SUB GETFUN (N, X(), F)
        '=====  FUNCTION EVALUATION
        F = 100 * (X(2) - X(1) ^ 2) ^ 2 + (1 - X(1)) ^ 2
        'F = (X(1) - X(2) + X(3)) ^ 2 + (X(1) + X(2) - X(3)) ^ 2 + (-X(1) + X(2) + X(3)) ^ 2
        'F = -3803.84 - 138.08 * X(1) - 232.92 * X(2)
        'F = F + 123.08 * X(1) ^ 2 + 203.64 * X(2) ^ 2 + 182.25 * X(1) * X(2)
        IF NFV < 30000 THEN NFV = NFV + 1
        END SUB

        SUB GETFUN1 (N, X0(), X(), D(), AL, F)
        FOR I = 1 TO N: X(I) = X0(I) + AL * D(I): NEXT I
        CALL GETFUN(N, X(), F)
        END SUB

        SUB QUADFIT (N, X0(), X(), D(), F1, XSTAR, FSTAR, STP)
        '--------------------------------------------------------------
        'Ref. An efficient quadratic fit- Sectioning algorithm for
        'minimization without derivatives,  T.R.CHANDRUPATLA
        'Comput. Methods in Appl. Mech. and Engrg. 152 (1998) 211-217
        '--------------------------------------------------------------
        X1 = 0
        CALL THREEPOINT(N, X1, X2, X3, F1, F2, F3, STP, X0(), X(), D())
        '--- 3-Point Pattern => F1 >= F2  and  F3 >= F2
        Q0 = X3
        EPSX = .0001: EPSF = .0001
    DO
        TOLF = 1E-16 * ABS(F2) + EPSF
        IF F1 - 2 * F2 + F3 < 2 * TOLF THEN EXIT DO
        '--- Make 3-2 as the larger interval
        IF ABS(X2 - X1) > ABS(X3 - X2) THEN
           C = X1: X1 = X3: X3 = C
           C = F1: F1 = F3: F3 = C
        END IF
        TOL = 1E-16 * ABS(X2) + EPSX
        IF ABS(X3 - X2) < 2 * TOL THEN EXIT DO
        '--- Fit Quadratic curve to 1-2-3
        AL = (F1 - F2) / ((F1 - F2) + (F3 - F2) * (X1 - X2) / (X2 - X3))
        Q1 = .5 * AL * (X2 + X3) + .5 * (1 - AL) * (X1 + X2)
        '--- Condition for accepting Quadratic minimum point
        IF ABS(Q1 - Q0) < .5 * ABS(X2 - X1) THEN
           '--- Accept Q1 adjust if close to X2
           IF ABS(Q1 - X2) > TOL THEN
              ALPHA = Q1
           ELSE
              ALPHA = X2 + SGN(X3 - X2) * TOL
           END IF
        ELSE
            '--- Golden section
            ALPHA = X2 + .381966 * (X3 - X2)
        END IF
        Q0 = Q1
        CALL GETFUN1(N, X0(), X(), D(), ALPHA, F)
        IF SGN(ALPHA - X2) = SGN(X3 - X2) THEN
           IF F > F2 THEN
              X3 = ALPHA: F3 = F
           ELSE
              X1 = X2: F1 = F2: X2 = ALPHA: F2 = F
           END IF
        ELSE
           IF F > F2 THEN
              X1 = ALPHA: F1 = F
           ELSE
              X3 = X2: F3 = F2: X2 = ALPHA: F2 = F
           END IF
        END IF
    LOOP
        XSTAR = X2
        FSTAR = F2
        END SUB

        SUB THREEPOINT (N, X1, X2, X3, F1, F2, F3, STP, X0(), X(), D())
        TAU = SQR(1.25) - .5
        ST = STP
        '----- Establish Three Point Pattern -----
        X2 = X1 + ST
        CALL GETFUN1(N, X0(), X(), D(), X2, F2)
        IF F2 > F1 THEN
            C = X1: X1 = X2: X2 = C
            C = F1: F1 = F2: F2 = C
            ST = -ST
        END IF
        DO
          ST = ST / TAU
          X3 = X2 + ST
          CALL GETFUN1(N, X0(), X(), D(), X3, F3)
          IF F3 >= F2 THEN
             EXIT DO
          ELSE
             F1 = F2: X1 = X2
             F2 = F3: X2 = X3
          END IF
        LOOP
        END SUB

