   M S B A M L   `   `   `  ����  EasySaveApp    EasySaveApp.RestoreWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Restaurer Sauvegarde��5   	   $	��700��6&   $	��600��63    � WindowStartupLocation$ CenterScreen=�5   	   ���#1E1E1E � WindowStyle$
 None=�5   	   .��5	      �� ���#2E2E2E �� CornerRadius$ 10��6"    �� Padding$ 10q�64   ���#444$��2q�6T   $��
0,0,0,-132q�6h   ._�5
   
   �  � RowDefinitions 5      �� 5      $��40 �6    �� 5      $��* �6    .+�5      � -   5        $��0��5      $��40��6    ���#444.+�5      �� $��Restaurer Sauvegarde��6   
�������$��Center=�5      $
��Left=�67   $��10q�5      $��16(�6(   $
��Bold"�66   �� 5      -   5        $��X��5      $��30��6%   $��30��60   $��Right=�6<   $��0,5,5,0q�6X   
������ 
�������$��0q�5   E   $
��Bold"�5      $��14(�6+   5      5      �� 5      $��1��6   $��Center=�6&   $	��Top=�6C   $��0,25,0,0q�6[   .+�5      �� $2��,Sélectionnez les sauvegardes à restaurer :��6   $��16(�6N   
�������$��10q�6o   �� 5       -   5         ��Name$ BackupListBox��5        �� SelectionMode$ Extended=�61   $	��400��6J   $	��200��6V   ���#3A3A3A
�������
�������$��1q�5!   U   �� 5#      $-��'Chemins de destination (un par ligne) :��6   $��14(�6K   
�������$��10q�6l   �� 5$      -   5         ��Name$ DestinationsTextBox��5$      $	��400��67   $	��100��6C   	 �� AcceptsReturn$
	 True��6P   ���#3A3A3A
�������
�������$��1q�5%   U   �� 5'      -   5        $-��'Ouvrir l'explorateur pour sélectionner��5'      $	��300��5(      $��40��6%   $��10q�61   ���#4A90E2
�������$
��Bold"�5)   A   $��0q�6S   �� 5+      $��Type de restauration :��6   $��14(�6:   
�������$��10q�6[   �� 5,      -   5        
 ��Name$
 RestoreTypeComboBox��5,      $	��200��68   ���#3A3A3A
���   ����#3A3A3A$��1q�5-   Y   .@�5.      �� $��	Complète��6#    �� 
IsSelected$
 True��66   �� 5/      $��Différentielle��6#   50      �� 52      -   5        $��Restaurer Sélection��52      $	��200��6O   $��40��6[   $��10q�6g   ���#4A90E2
�������$
��Bold"�53   A   $��0q�6S   54      55      56      57      