   M S B A M L   `   `   `  ����N  JCyberSecurityPoeWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:CyberSecurityPoeWPFCyberSecurityPoeWPF  5   	   $    CyberSecurityPoeWPF.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:CyberSecurityPoeWPF     5   	     � Title$  Cyber Awareness Hub��5   	   $	��650��6%   $	��900��62   $	��400��6>   $	��600��6N   .��5	      � $��10q�6    � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ rtbConversation��5      $��0��6/   $��0,0,0,10q�6<    �� 
IsReadOnly$
 True��6N   ���#F0F8FF���#ADD8E6$��1q�5   A   $��8q�6U   " �� VerticalScrollBarVisibility$
 Auto=�5      . �5      /� 5      5      ]� 5   
   $��1��6   $
��True��6!   $��0,0,0,10q�66   .+�5      �� -   5         ��Name$ btnSend��5      $
��Send��6&   $��Right=�65   $��80��6L   $��30��6W   $��5,0,0,0q�6c   6t   �� 5      -   5         ��Name$ txtInput��5       �� TextWrapping$
 Wrap=�6(    �� AcceptsReturn$ False��6<   $��30��6R   $��5q�6^   5      5      � 5   
   $��2��6   $��Center=�6!   $��0,0,0,0q�6>   .+�5      �� -   5        $ btnMenu��5      $
��Menu��6&   $	��100��65   $��30��6A   $��5q�6M   6X   �� 5      -   5        $ btnStartQuiz��5      $��
Start Quiz��6+   $	��100��6@   $��30��6L   $��5q�6X   6c   �� 5       -   5        $ 
btnAddTask��5       $��Add Task��6)   $	��100��6<   $��30��6H   $��5q�6T   6_   �� 5!      -   5        $ btnShowTasks��5!      $��
Show Tasks��6+   $	��100��6@   $��30��6L   $��5q�6X   6c   �� 5"      -   5        $ btnCompleteTask��5"      $��Complete Task��6.   $	��120��6F   $��30��6R   $��5q�6^   6i   �� 5#      -	   5        $ 
btnShowLog��5#      $��Show Activity Log��6)   $	��130��6E   $��30��6Q   $��5q�6]   6h   �� 5$      -
   5        $ btnExit��5$      $
��Exit��6&   $	��100��65   $��30��6A   $��5q�6M   6X   5%      5&      5'      