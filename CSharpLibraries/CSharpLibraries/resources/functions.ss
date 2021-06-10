(define combine (lambda (f)
    (lambda (x y)
      (if (null? x) (quote ())
          (f (list (car x) (car y))
             ((combine f) (cdr x) (cdr y)))))))

(define zip (combine cons))

(define riff-shuffle (lambda (deck) (begin
    (define take (lambda (n seq) (if (<= n 0) (quote ()) (cons (car seq) (take (- n 1) (cdr seq))))))
    (define drop (lambda (n seq) (if (<= n 0) seq (drop (- n 1) (cdr seq)))))
    (define mid (lambda (seq) (/ (length seq) 2)))
    ((combine append) (take (mid deck) deck) (drop (mid deck) deck)))))

(define (twice x) (* 2 x))

(define compose (lambda (f g) (lambda (x) (f (g x)))))

(define repeat (lambda (f) (compose f f)))

(define fact (lambda (n) (if (<= n 1) 1 (* n (fact (- n 1))))))

(define lyst (lambda items items))

(define ((account bal) amt) (set! bal (+ bal amt)) bal)

(define (newton guess function derivative epsilon)
    (define guess2 (- guess (/ (function guess) (derivative guess))))
    (if (< (abs (- guess guess2)) epsilon) guess2
        (newton guess2 function derivative epsilon)))

(define (square-root a)
    (newton 1 (lambda (x) (- (* x x) a)) (lambda (x) (* 2 x)) 1e-8))

(define (sum-squares-range start end)
        (define (sumsq-acc start end acc)
            (if (> start end) acc (sumsq-acc (+ start 1) end (+ (* start start) acc))))
         (sumsq-acc start end 0))

(define fact (lambda (n) (if (<= n 1) 1 (* n (fact (- n 1))))))

(define fib (lambda (n) (if (< n 2) 1 (+ (fib (- n 1)) (fib (- n 2))))))

(define count
        (lambda (item L)
            (if (null? L) 0 (+ (if (equal? item (car L)) 1 0) (count item (cdr L))))))

(define square (lambda (x) (* x x)))

(define range (lambda (a b) (if (= a b) nil (cons a (range (+ a 1) b)))))


(define (sum2 n acc)
    (if (= n 0)
        acc
        (sum2 (- n 1) (+ n acc))))

(define (cube x) (* x x x))

(define abs (lambda (n) ((if (> n 0) + -) 0 n)))

(define-macro unless (lambda args `(if (not ,(car args)) (begin ,@(cdr args))))) ;  test `