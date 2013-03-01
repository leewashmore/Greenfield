U S E   [ A I M S _ M a i n ]  
 G O  
 / * * * * * *   O b j e c t :     S t o r e d P r o c e d u r e   [ d b o ] . [ A I M S _ C a l c _ 2 3 6 ]         S c r i p t   D a t e :   0 3 / 0 1 / 2 0 1 3   0 8 : 1 5 : 2 8   * * * * * * /  
 S E T   A N S I _ N U L L S   O N  
 G O  
 S E T   Q U O T E D _ I D E N T I F I E R   O F F  
 G O  
 A L T E R   p r o c e d u r e   [ d b o ] . [ A I M S _ C a l c _ 2 3 6 ] (  
 	 @ I S S U E R _ I D 	 	 	 v a r c h a r ( 2 0 )   =   N U L L 	 	 	 - -   T h e   c o m p a n y   i d e n t i f i e r 	 	  
 , 	 @ C A L C _ L O G 	 	 	 c h a r 	 	 =   ' Y ' 	 	 	 - -   w r i t e   c a l c u l a t i o n   e r r o r s   t o   t h e   l o g   t a b l e  
 )  
 a s  
 	  
 	 - -   G e t   t h e   d a t a  
 	 s e l e c t   d i s t i n c t   p f . *    
 	     i n t o   # A  
 	     f r o m   d b o . P E R I O D _ F I N A N C I A L S   p f     w i t h   ( n o l o c k )  
 	   i n n e r   j o i n   d b o . G F _ S E C U R I T Y _ B A S E V I E W   s b   o n   s b . S E C U R I T Y _ I D   =   p f . S E C U R I T Y _ I D  
 	   w h e r e   D A T A _ I D   =   1 8 5 	 	 	 - - M a r k e t   C a p i t a l i z a t i o n  
 	       a n d   s b . I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   p f . P E R I O D _ T Y P E   =   ' C '  
 	        
   - - - -   T o t a l   a m o u n t   f o r   a l l   t h e   f i s c a l   q u a r t e r s   w i t h i n   a n   y e a r   - - -    
  
 	 - -   C a l c u l a t e   t h e   p e r c e n t a g e   o f   t h e   a m o u n t   t o   u s e .  
 	 d e c l a r e   @ P E R C E N T A G E   d e c i m a l ( 3 2 , 6 )  
 	 d e c l a r e   @ P E R I O D _ E N D _ D A T E   d a t e t i m e  
 	 d e c l a r e   @ P E R I O D _ Y E A R   i n t e g e r  
 	 s e l e c t   @ P E R C E N T A G E   =   c a s t ( d a t e d i f f ( d a y ,   g e t d a t e ( ) ,   M I N ( p e r i o d _ e n d _ d a t e ) )   a s   d e c i m a l ( 3 2 , 6 ) )   /   3 6 5 . 0  
 	       ,       @ P E R I O D _ E N D _ D A T E   =   M I N ( p e r i o d _ e n d _ d a t e )  
 	     f r o m   P E R I O D _ F I N A N C I A L S   w i t h   ( n o l o c k )  
 	   w h e r e   I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   D A T A _ I D   =   1 2 4  
 	       a n d   P E R I O D _ E N D _ D A T E   >   G E T D A T E ( )  
 	       a n d   P E R I O D _ T Y P E   =   ' A '  
  
 	 s e l e c t   @ P E R I O D _ Y E A R   =   P E R I O D _ Y E A R  
 	     f r o m   P E R I O D _ F I N A N C I A L S   w i t h   ( n o l o c k )  
 	   w h e r e   I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   D A T A _ I D   =   1 2 4  
 	       a n d   P E R I O D _ E N D _ D A T E   =   @ P E R I O D _ E N D _ D A T E  
 	       a n d   P E R I O D _ T Y P E   =   ' A '  
        
 	 s e l e c t   d i s t i n c t   I S S U E R _ I D ,   S E C U R I T Y _ I D ,   C O A _ T Y P E ,   D A T A _ S O U R C E ,   R O O T _ S O U R C E  
 	 	     ,   R O O T _ S O U R C E _ D A T E ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y  
 	 	     ,   D A T A _ I D ,   C A L C U L A T I O N _ D I A G R A M ,   S O U R C E _ C U R R E N C Y ,   A M O U N T _ T Y P E  
 	 	     ,   ( A M O U N T   *   @ P E R C E N T A G E ) * - 1   a s   A M O U N T ,   - 1 * A M O U N T   a s   A M O U N T 1 0 0  
 	     i n t o   # B  
 	     f r o m   d b o . P E R I O D _ F I N A N C I A L S   p f     w i t h   ( n o l o c k )  
 	   w h e r e   D A T A _ I D   =   1 2 4 	 	 - -   D i v i d e n d s  
 	       a n d   p f . I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   p f . P E R I O D _ T Y P E   =   ' A '  
 	       a n d   F I S C A L _ T Y P E   =   ' F I S C A L '  
 	       a n d   p f . P E R I O D _ E N D _ D A T E   =   @ P E R I O D _ E N D _ D A T E  
 	  
 	 s e l e c t   d i s t i n c t   I S S U E R _ I D ,   S E C U R I T Y _ I D ,   C O A _ T Y P E ,   D A T A _ S O U R C E ,   R O O T _ S O U R C E  
 	 	     ,   R O O T _ S O U R C E _ D A T E ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y  
 	 	     ,   D A T A _ I D ,   C A L C U L A T I O N _ D I A G R A M ,   S O U R C E _ C U R R E N C Y ,   A M O U N T _ T Y P E  
 	 	     ,   ( A M O U N T   *   ( 1 - @ P E R C E N T A G E ) ) * - 1   a s   A M O U N T  
 	     i n t o   # C  
 	     f r o m   d b o . P E R I O D _ F I N A N C I A L S   p f     w i t h   ( n o l o c k )  
 	   w h e r e   D A T A _ I D   =   1 2 4 	 	 - -   D i v i d e n d s  
 	       a n d   p f . I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   p f . P E R I O D _ T Y P E   =   ' A '  
 	       a n d   F I S C A L _ T Y P E   =   ' F I S C A L '  
 	       a n d   p f . P E R I O D _ Y E A R   =   @ P E R I O D _ Y E A R   +   1  
  
 	 - -   A d d   t h e   d a t a   t o   t h e   t a b l e  
 	 - -   W h e n   d e a l i n g   w i t h   ' C ' u r r e n t   P E R I O D _ T Y P E   t h e r e   s h o u l d   b e   o n l y   o n e   v a l u e . . .   t h e   c u r r e n t   o n e .      
 	 - -   N o   P E R I O D _ Y E A R   n o t   P E R I O D _ E N D _ D A T E   i s   n e e d e d .  
 	 B E G I N   T R A N   T 1  
 	 i n s e r t   i n t o   P E R I O D _ F I N A N C I A L S ( I S S U E R _ I D ,   S E C U R I T Y _ I D ,   C O A _ T Y P E ,   D A T A _ S O U R C E ,   R O O T _ S O U R C E  
 	 	     ,   R O O T _ S O U R C E _ D A T E ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y  
 	 	     ,   D A T A _ I D ,   A M O U N T ,   C A L C U L A T I O N _ D I A G R A M ,   S O U R C E _ C U R R E N C Y ,   A M O U N T _ T Y P E )  
 	 s e l e c t   d i s t i n c t   a . I S S U E R _ I D ,   a . S E C U R I T Y _ I D ,   a . C O A _ T Y P E ,   a . D A T A _ S O U R C E ,   a . R O O T _ S O U R C E  
 	 	 ,     a . R O O T _ S O U R C E _ D A T E ,   ' C ' ,   0 ,   ' 0 1 / 0 1 / 1 9 0 0 ' 	 	 	 	 - -   T h e s e   a r e   s p e c i f i c   f o r   P E R I O D _ T Y P E   =   ' C '  
 	 	 ,     a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 ,     2 3 6   a s   D A T A _ I D 	 	 	 	 	 	 	 	 	 	 - -   D A T A _ I D : 2 3 6   F o r w a r d   D i v i d e n d   Y i e l d  
 	 	 ,     c a s e   w h e n   c . A M O U N T   i s   N U L L   t h e n   b . A M O U N T 1 0 0   e l s e   ( b . A M O U N T   + c . A M O U N T )   e n d   / a . A M O U N T   a s   A M O U N T 	   	 	 	 	 	 - -     S u m   o f   n e x t   4   q u a r t e r s   F C D P * * / C u r r e n t   M a r k e t   C a p *    
 	 	 ,     c a s e   w h e n   c . A M O U N T   i s   N U L L   t h e n     ' D i v i d e n d s ( '   +   C A S T ( b . A M O U N T 1 0 0   a s   v a r c h a r ( 3 2 ) )   +   ' )   /   M k t c a p ( '   +   C A S T ( a . A M O U N T   a s   v a r c h a r ( 3 2 ) )   +   ' ) '    
 	 	 	 	 e l s e   ' D i v i d e n d s ( '   +   C A S T ( b . A M O U N T   a s   v a r c h a r ( 3 2 ) )   +   '   +   '   + C A S T ( c . A M O U N T   a s   v a r c h a r ( 3 2 ) )   +   ' )   /   M k t c a p ( '   +   C A S T ( a . A M O U N T   a s   v a r c h a r ( 3 2 ) )   +   ' ) '   e n d   a s   C A L C U L A T I O N _ D I A G R A M  
 	 	 ,     a . S O U R C E _ C U R R E N C Y  
 	 	 ,     a . A M O U N T _ T Y P E  
 	     f r o m   # A   a  
 	   i n n e r   j o i n   d b o . G F _ S E C U R I T Y _ B A S E V I E W   s b   o n   s b . S E C U R I T Y _ I D   =   a . S E C U R I T Y _ I D  
 	   i n n e r   j o i n 	 # B   b   o n   b . I S S U E R _ I D   =   s b . I S S U E R _ I D   	 	 	 	 	  
 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E  
 	     l e f t   j o i n 	 # C   c   o n   c . I S S U E R _ I D   =   s b . I S S U E R _ I D   	 	 	 	 	  
 	 	 	 	 	 a n d   c . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	 	 	 a n d   c . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E  
 	   w h e r e   1 = 1   	      
 	       a n d   i s n u l l ( b . A M O U N T ,   0 . 0 )   < >   0 . 0 	 - -   D a t a   v a l i d a t i o n  
 - - 	       a n d   i s n u l l ( c . A M O U N T ,   0 . 0 )   < >   0 . 0 	 - -   D a t a   v a l i d a t i o n  
 	 - -   o r d e r   b y   a . I S S U E R _ I D ,   a . C O A _ T Y P E ,   a . D A T A _ S O U R C E ,   a . P E R I O D _ T Y P E ,   a . P E R I O D _ Y E A R ,     a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 C O M M I T   T R A N   T 1  
  
 	  
 	 i f   @ C A L C _ L O G   =   ' Y '  
 	 	 B E G I N  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   N U L L   o r   Z e r o   d a t a    
 	 	 	 i n s e r t   i n t o   C A L C _ L O G (   L O G _ D A T E ,   D A T A _ I D ,   I S S U E R _ I D ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R  
 	 	 	 	 	 	 	 ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y ,   T X T   )  
 	 	 	 (  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   2 3 6   a s   D A T A _ I D ,   a . I S S U E R _ I D ,   ' C '  
 	 	 	 	 ,     0 ,   ' 0 1 / 0 1 / 1 9 0 0 ' ,   a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   2 3 6   F o r w a r d   D i v i d e n d   Y i e l d .   D A T A _ I D : 1 2 4   i s   N U L L   o r   Z E R O '  
 	 	 	     f r o m   # B   a  
 	 	 	   w h e r e   i s n u l l ( a . A M O U N T ,   0 . 0 )   =   0 . 0 	   - -   D a t a   e r r o r 	    
 	 	 	 )   u n i o n   ( 	  
 	 	 	  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   m i s s i n g   d a t a    
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   2 3 6   a s   D A T A _ I D ,   s b . I S S U E R _ I D ,   ' C '  
 	 	 	 	 ,     0 ,   ' 0 1 / 0 1 / 1 9 0 0 ' ,   a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   2 3 6   F o r w a r d   D i v i d e n d   Y i e l d   .     D A T A _ I D : 1 8 5   i s   m i s s i n g '   a s   T X T  
 	 	 	     f r o m   # B   a  
 	 	 	   i n n e r   j o i n   d b o . G F _ S E C U R I T Y _ B A S E V I E W   s b   o n   s b . I S S U E R _ I D   =   a . I S S U E R _ I D  
 	 	 	   l e f t   j o i n   # A   b   o n     b . S E C U R I T Y _ I D   =   s b . S E C U R I T Y _ I D  
 	 	 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E  
 	 	 	   w h e r e   1 = 1   a n d   b . I S S U E R _ I D   i s   N U L L 	      
 	 	 	 )   u n i o n   ( 	  
  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   m i s s i n g   d a t a    
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   2 3 6   a s   D A T A _ I D ,   s b . I S S U E R _ I D ,   ' C '  
 	 	 	 	 ,     0 ,   ' 0 1 / 0 1 / 1 9 0 0 ' ,   a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   2 3 6   F o r w a r d   D i v i d e n d   Y i e l d   .     D A T A _ I D : 1 2 4   i s   m i s s i n g '   a s   T X T  
 	 	 	     f r o m   # A   a  
 	 	 	   i n n e r   j o i n   d b o . G F _ S E C U R I T Y _ B A S E V I E W   s b   o n   s b . S E C U R I T Y _ I D   =   a . S E C U R I T Y _ I D  
 	 	 	   l e f t   j o i n   # B   b   o n   b . I S S U E R _ I D   =   s b . I S S U E R _ I D   	 	 	 	 	  
 	 	 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E  
 	 	 	   w h e r e   1 = 1   a n d   b . I S S U E R _ I D   i s   N U L L 	      
 	 	 	 )   u n i o n   ( 	  
 	 	 	  
 	 	 	 	 - -   E R R O R   -   N o   d a t a   a t   a l l   a v a i l a b l e  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   2 3 6   a s   D A T A _ I D ,   i s n u l l ( @ I S S U E R _ I D ,   '   ' )   a s   I S S U E R _ I D ,   '   '   a s   P E R I O D _ T Y P E  
 	 	 	 	 ,     0   a s   P E R I O D _ Y E A R ,     ' 1 / 1 / 1 9 0 0 '   a s   P E R I O D _ E N D _ D A T E ,     '   '   a s   F I S C A L _ T Y P E ,     '   '   a s   C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   2 3 6   F o r w a r d   D i v i d e n d   Y i e l d   .     D A T A _ I D : 1 8 5   n o   d a t a '   a s   T X T  
 	 	 	     f r o m   ( s e l e c t   C O U N T ( * )   C N T   f r o m   # A   h a v i n g   C O U N T ( * )   =   0 )   z  
 	 	 	 )   u n i o n   ( 	  
  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   2 3 6   a s   D A T A _ I D ,   i s n u l l ( @ I S S U E R _ I D ,   '   ' )   a s   I S S U E R _ I D ,   '   '   a s   P E R I O D _ T Y P E  
 	 	 	 	 ,     0   a s   P E R I O D _ Y E A R ,     ' 1 / 1 / 1 9 0 0 '   a s   P E R I O D _ E N D _ D A T E ,     '   '   a s   F I S C A L _ T Y P E ,     '   '   a s   C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   2 3 6   F o r w a r d   D i v i d e n d   Y i e l d .     D A T A _ I D : 1 2 4   N o   d a t a   o r   m i s s i n g   q u a r t e r s '   a s   T X T  
 	 	 	     f r o m   ( s e l e c t   C O U N T ( * )   C N T   f r o m   # B   h a v i n g   C O U N T ( * )   =   0 )   z  
 	 	 	 )  
 	 	 E N D  
  
  
 	 - -   C l e a n   u p  
 	 d r o p   t a b l e   # A  
 	 d r o p   t a b l e   # B  
 	 d r o p   t a b l e   # C  
 