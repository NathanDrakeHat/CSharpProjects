(quote (testing 1 (2.0) -3.14e159)) ; => (testing 1 (2.0) -3.14e+159)

(+ 2 2) ; => 4

(+ (* 2 100) (* 1 10));  => 210

(if (> 6 5) (+ 1 1) (+ 2 2));  => 2

(if (< 6 5) (+ 1 1) (+ 2 2));  => 4

(define x 3)

x ; => 3

(+ x x);  => 6

(begin (define x 1) (set! x (+ x 1)) (+ x 1)) ; => 3

((lambda (x) (+ x x)) 5) ; => 10

(twice 5) ; => 10

((compose list twice) 5) ; => (10)

((repeat twice) 5) ; => 20

((repeat (repeat twice)) 5) ; => 80

(fact 3) ; => 6

(fact 10) ; => 3628800

(list (abs -3) (abs 0) (abs 3)) ; => (3 0 3)

(zip (list 1 2 3 4) (list 5 6 7 8)) ; => ((1 5) (2 6) (3 7) (4 8))

(riff-shuffle (list 1 2 3 4 5 6 7 8)) ; => (1 5 2 6 3 7 4 8)

((repeat riff-shuffle) (list 1 2 3 4 5 6 7 8)) ; => (1 3 5 7 2 4 6 8)

(riff-shuffle (riff-shuffle (riff-shuffle (list 1 2 3 4 5 6 7 8)))) ; => (1 2 3 4 5 6 7 8)

() ;=raises=> SyntaxError (): wrong length

(set! x) ; =raises=> SyntaxError (set! x): wrong length

(define 3 4) ; =raises=> SyntaxError (define 3 4): can define only a symbol

(quote 1 2) ; =raises=> SyntaxError (quote 1 2): wrong length

(if 1 2 3 4) ; =raises; => SyntaxError (if 1 2 3 4): wrong length

(lambda 3 3) ; =raises=> SyntaxError (lambda 3 3): illegal lambda argument list

(lambda (x)) ; =raises=> SyntaxError (lambda (x)): wrong length

(if (= 1 2) (define-macro a 'a)
    (define-macro a 'b)) ; =raises=> SyntaxError (define-macro a (quote a)): define-macro only allowed at top level

(twice 2) ; => 4

(twice 2 2) ; =raises=> TypeError expected (x), given (2 2)

(lyst 1 2 3 (+ 2 2)) ; => (1 2 3 4)

(if 1 2) ; => 2

(if (= 3 4) 2)

(define ((account bal) amt) (set! bal (+ bal amt)) bal)

(define a1 (account 100))

(a1 0) ; => 100

(a1 10) ; => 110

(a1 10) ; => 120

(> (square-root 200.) 14.14213) ; => #t

(< (square-root 200.) 14.14215) ; => #t

(= (square-root 200.) (sqrt 200.)) ; => #t

(sum-squares-range 1 300) ; => 9045050

(call/cc (lambda (throw) (+ 5 (* 10 (throw 1))))) ; throw ; => 1


(call/cc (lambda (throw) (+ 5 (* 10 1)))) ;; do not throw ; => 15

(call/cc (lambda (throw)
         (+ 5 (* 10 (call/cc (lambda (escape) (* 100 (escape 3)))))))) ;  1 level ; => 35

(call/cc (lambda (throw)
         (+ 5 (* 10 (call/cc (lambda (escape) (* 100 (throw 3)))))))) ;  2 levels ; => 3

(call/cc (lambda (throw)
         (+ 5 (* 10 (call/cc (lambda (escape) (* 100 1))))))) ;  0 levels ; => 1005

(* 1i 1i) ; => (-1+0i)

(sqrt -1) ; => 1i ;

(let ((a 1) (b 2)) (+ a b)) ; => 3

(let ((a 1) (b 2 3)) (+ a b)) ; =raises=> SyntaxError (let ((a 1) (b 2 3)) (+ a b)): illegal binding list

(and 1 2 3) ; => 3

(and (> 2 1) 2 3) ; => 3

(and) ; => #t

(and (> 2 1) (> 2 3)) ; => #f

(unless (= 2 (+ 1 1)) (display 2) 3 4) ; => None

2 ; => 2

(unless (= 4 (+ 1 1)) (display 2) (display "\n") 3 4) ; => 4

(quote x) ; => x

(quote (1 2 three)) ; => (1 2 three)

'x ; => x

'(one 2 3) ; => (one 2 3)

(define L (list 1 2 3)) ; => None

`(testing ,@L testing) ; => (testing 1 2 3 testing)

`(testing ,L testing) ; => (testing (1 2 3) testing)

`,@L ;  =raises=> SyntaxError (unquote-splicing L): can't splice here

'(1 ; test comments '
     ; skip this line
     2 ;  more ;  comments ;  ) )
     3) ;  final comment  ; => (1 2 3)