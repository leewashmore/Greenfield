S E T   A N S I _ N U L L S   O N  
 G O  
 S E T   Q U O T E D _ I D E N T I F I E R   O F F  
 G O  
 a l t e r   p r o c e d u r e   [ d b o ] . [ A I M S _ C a l c _ 1 7 4 ] (  
 	 @ I S S U E R _ I D 	 	 	 v a r c h a r ( 2 0 )   =   N U L L 	 	 	 - -   T h e   c o m p a n y   i d e n t i f i e r 	 	  
 , 	 @ C A L C _ L O G 	 	 	 c h a r 	 	 =   ' Y ' 	 	 	 - -   w r i t e   c a l c u l a t i o n   e r r o r s   t o   t h e   l o g   t a b l e  
 )  
 a s  
  
 	 - -   G e t   t h e   d a t a  
 	 s e l e c t   p f . *    
 	     i n t o   # A  
 	   f r o m   d b o . P E R I O D _ F I N A N C I A L S   p f   w i t h   ( n o l o c k )  
 	   w h e r e   D A T A _ I D   =   7 5 	 	 	 - - A T O T  
 	       a n d   p f . I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   p f . P E R I O D _ T Y P E   =   ' A '  
 	   o r d e r   b y   P E R I O D _ Y E A R  
  
 	 s e l e c t   p f . P E R I O D _ Y E A R   +   1   a s   P R I O R _ Y E A R ,   p f . P E R I O D _ Y E A R   a s   P E R I O D _ Y E A R , p f . C U R R E N C Y , p f . C O A _ T Y P E , p f . D A T A _ S O U R C E , p f . F I S C A L _ T Y P E , p f . P E R I O D _ T Y P E , p f . I S S U E R _ I D , p f . A M O U N T , p f . P E R I O D _ E N D _ D A T E  
 	     i n t o   # B  
 	     f r o m   d b o . P E R I O D _ F I N A N C I A L S   p f   w i t h   ( n o l o c k )  
 	   w h e r e   D A T A _ I D   =   7 5 	 	 	 - - A T O T  
 	       a n d   p f . I S S U E R _ I D   =   @ I S S U E R _ I D  
 	       a n d   p f . P E R I O D _ T Y P E   =   ' A '  
 	   o r d e r   b y   P E R I O D _ Y E A R  
  
 	 - -   A d d   t h e   d a t a   t o   t h e   t a b l e  
 	 B E G I N   T R A N   T 1  
 	 i n s e r t   i n t o   P E R I O D _ F I N A N C I A L S ( I S S U E R _ I D ,   S E C U R I T Y _ I D ,   C O A _ T Y P E ,   D A T A _ S O U R C E ,   R O O T _ S O U R C E  
 	 	     ,   R O O T _ S O U R C E _ D A T E ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y  
 	 	     ,   D A T A _ I D ,   A M O U N T ,   C A L C U L A T I O N _ D I A G R A M ,   S O U R C E _ C U R R E N C Y ,   A M O U N T _ T Y P E )  
 	 s e l e c t   a . I S S U E R _ I D ,   a . S E C U R I T Y _ I D ,   a . C O A _ T Y P E ,   a . D A T A _ S O U R C E ,   a . R O O T _ S O U R C E  
 	 	 ,     a . R O O T _ S O U R C E _ D A T E ,   a . P E R I O D _ T Y P E ,   a . P E R I O D _ Y E A R ,   a . P E R I O D _ E N D _ D A T E  
 	 	 ,     a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 ,     1 7 4   a s   D A T A _ I D 	 	 	 	 	 	 	 	 	 	 - -   D A T A _ I D : 1 7 4   A s s e t s   G r o w t h   ( Y O Y )    
 	 	 ,     C A S E   W H E N   a . A M O U N T   > =   0   a n d   b . A M O U N T   >   0   T H E N   ( a . A M O U N T   / b . A M O U N T ) - 1    
 	 	 	 	 E L S E   N U L L    
 	 	 	 	 E N D   a s   A M O U N T 	 	 	 	 	 - -   ( A T O T   f o r   Y e a r /   A T O T   f o r   P r i o r   Y e a r ) - 1  
 	 	 ,     ' ( A T O T   f o r   ( ' +   C A S T ( a . P E R I O D _ Y E A R   a s   v a r c h a r ( 3 2 ) )   +   ' ( '   +   C A S T ( a . A M O U N T   a s   v a r c h a r ( 3 2 ) )   +   ' )   /   A T O T   f o r   ( ' +   C A S T ( b . P E R I O D _ Y E A R   a s   v a r c h a r ( 3 2 ) )   + ' ) ( '   +   C A S T ( b . A M O U N T   a s   v a r c h a r ( 3 2 ) )   +   ' ) )   -   1   '   a s   C A L C U L A T I O N _ D I A G R A M  
 	 	 ,     a . S O U R C E _ C U R R E N C Y  
 	 	 ,     a . A M O U N T _ T Y P E  
 	     f r o m   # A   a  
 	   i n n e r   j o i n 	 # B   b   o n   b . I S S U E R _ I D   =   a . I S S U E R _ I D    
 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E   a n d   b . P E R I O D _ T Y P E   =   a . P E R I O D _ T Y P E  
 	 	 	 	 	 a n d   b . P R I O R _ Y E A R   =   a . P E R I O D _ Y E A R   a n d   b . F I S C A L _ T Y P E   =   a . F I S C A L _ T Y P E  
 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	   w h e r e   1 = 1    
 	       a n d   a . P E R I O D _ T Y P E   =   ' A '  
 	     a n d    
 	     	   i s n u l l ( a . A M O U N T ,   0 . 0 ) > = 0     a n d   i s n u l l ( b . A M O U N T ,   0 . 0 )   >   0 . 0 - -   D a t a   v a l i d a t i o n  
 - - 	   o r d e r   b y   a . I S S U E R _ I D ,   a . C O A _ T Y P E ,   a . D A T A _ S O U R C E ,   a . P E R I O D _ T Y P E ,   a . P E R I O D _ Y E A R ,     a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 C O M M I T   T R A N   T 1  
 	 i f   @ C A L C _ L O G   =   ' Y '  
 	 	 B E G I N 	  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   N U L L   o r   Z e r o   d a t a    
 	 	 	 i n s e r t   i n t o   C A L C _ L O G (   L O G _ D A T E ,   D A T A _ I D ,   I S S U E R _ I D ,   P E R I O D _ T Y P E ,   P E R I O D _ Y E A R  
 	 	 	 	 	 	 	 ,   P E R I O D _ E N D _ D A T E ,   F I S C A L _ T Y P E ,   C U R R E N C Y ,   T X T   )  
 	 	 	 (  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 7 4   a s   D A T A _ I D ,   a . I S S U E R _ I D ,   a . P E R I O D _ T Y P E  
 	 	 	 	 ,     a . P E R I O D _ Y E A R ,   a . P E R I O D _ E N D _ D A T E ,   a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   1 7 4   A s s e t s   G r o w t h   ( Y O Y )   .     D A T A _ I D : 7 5   i s   N U L L   o r   Z E R O   f o r   p r i o r   p e r i o d '  
 	 	 	     f r o m   # B   a  
 	 	 	   w h e r e   i s n u l l ( a . A M O U N T ,   0 . 0 )   =   0 . 0 	   - -   D a t a   e r r o r  
 	 	 	   a n d   a . P E R I O D _ T Y P E   =   ' A '  
 	 	 	 )   u n i o n   ( 	  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   m i s s i n g   d a t a    
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 7 4   a s   D A T A _ I D ,   a . I S S U E R _ I D ,   a . P E R I O D _ T Y P E  
 	 	 	 	 ,     a . P E R I O D _ Y E A R ,   a . P E R I O D _ E N D _ D A T E ,   a . F I S C A L _ T Y P E ,   a . C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   1 7 4   A s s e t s   G r o w t h   ( Y O Y )   .     D A T A _ I D : 7 5   i s   m i s s i n g   f o r   t h e   y e a r '   a s   T X T  
 	 	 	     f r o m   # B   a  
 	 	 	     l e f t   j o i n 	 # A   b   o n   b . I S S U E R _ I D   =   a . I S S U E R _ I D    
 	 	 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E   a n d   b . P E R I O D _ T Y P E   =   a . P E R I O D _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . P E R I O D _ Y E A R   =   a . P R I O R _ Y E A R   a n d   b . F I S C A L _ T Y P E   =   a . F I S C A L _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	   w h e r e   1 = 1   a n d   b . I S S U E R _ I D   i s   N U L L  
 	 	 	       a n d   a . P E R I O D _ T Y P E   =   ' A '  
 	 	 	 )   u n i o n   ( 	  
  
 	 	 	 - -   E r r o r   c o n d i t i o n s   -   m i s s i n g   d a t a   f o r   p r i o r   p e r i o d  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 7 4   a s   D A T A _ I D ,   a . I S S U E R _ I D ,   a . P E R I O D _ T Y P E  
 	 	 	 	 ,     a . P E R I O D _ Y E A R ,     a . P E R I O D _ E N D _ D A T E ,     a . F I S C A L _ T Y P E ,     a . C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   1 7 4   A s s e t s   G r o w t h   ( Y O Y )   .     D A T A _ I D : 7 5   i s   m i s s i n g   f o r   t h e   p r i o r   p e r i o d '   a s   T X T  
 	 	 	     f r o m   # A   a  
 	 	 	     l e f t   j o i n 	 # B   b   o n   b . I S S U E R _ I D   =   a . I S S U E R _ I D    
 	 	 	 	 	 	 	 a n d   b . D A T A _ S O U R C E   =   a . D A T A _ S O U R C E   a n d   b . P E R I O D _ T Y P E   =   a . P E R I O D _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . P R I O R _ Y E A R   =   a . P E R I O D _ Y E A R   a n d   b . F I S C A L _ T Y P E   =   a . F I S C A L _ T Y P E  
 	 	 	 	 	 	 	 a n d   b . C U R R E N C Y   =   a . C U R R E N C Y  
 	 	 	   w h e r e   1 = 1   a n d   b . I S S U E R _ I D   i s   N U L L  
 	 	 	       a n d   a . P E R I O D _ T Y P E   =   ' A '  
 	 	 	 )   u n i o n   ( 	  
 	 	 	    
 	 	 	 - -   E R R O R   -   N o   d a t a   a t   a l l   a v a i l a b l e  
 	 	 	 s e l e c t   G E T D A T E ( )   a s   L O G _ D A T E ,   1 7 4   a s   D A T A _ I D ,   i s n u l l ( @ I S S U E R _ I D ,   '   ' )   a s   I S S U E R _ I D ,   '   '   a s   P E R I O D _ T Y P E  
 	 	 	 	 ,     0   a s   P E R I O D _ Y E A R ,     ' 1 / 1 / 1 9 0 0 '   a s   P E R I O D _ E N D _ D A T E ,     '   '   a s   F I S C A L _ T Y P E ,     '   '   a s   C U R R E N C Y  
 	 	 	 	 ,   ' E R R O R   c a l c u l a t i n g   1 7 4   A s s e t s   G r o w t h   ( Y O Y )   .     D A T A _ I D : 7 5   n o   d a t a '   a s   T X T  
 	 	 	     f r o m   ( s e l e c t   C O U N T ( * )   C N T   f r o m   # A   h a v i n g   C O U N T ( * )   =   0 )   z  
 	 	 	 )    
 	 	 E N D  
 	 	  
 	 - -   C l e a n   u p  
 	 d r o p   t a b l e   # A  
 	 d r o p   t a b l e   # B  
 G O  
 