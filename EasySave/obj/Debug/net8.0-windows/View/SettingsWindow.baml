   M S B A M L   `   `   `  ����F  BEasySaveApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ViewModels
ViewModels  5	   	       Views.SettingsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   'vmclr-namespace:ViewModels     5	   	     � Title$  Paramètres��5   	    � SizeToContent$ WidthAndHeight=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   
���   � � WindowStyle$
 None=�5   	   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��250 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   ���#2E2E2E �� Padding$ 10q�5      ���#444$��2q�5      ._�5      �� $��Stretch=�6   $	��Top=�67   .+�5      �� $��Paramètres��6   $��24(�5      $
��Bold"�5      
�������$��10q�5      $��Center=�5      �� 5      $��Retour��6   ���#4A90E2
�������$��40��5!      $��5q�5"      ��5      �� *RetourCommand+5"      5#      5$      �� 5&   
   $��1��6   $ 20q�5'      ���#444$��2q�5)      ��5*      �� 5+       �� 
StartPoint$	 0,0&�6&    �� EndPoint$	 1,1&�67   .��5,      �  � Color$ #4A90E2��6#   	 � Offset$	 0Q�63   � 5-      $ #1E1E1E��6#   $	 1Q�63   5.      ._�51      �� .+�52      �� $��Crypter Fichier��6   $��20(�53      $
��Bold"�54      
�������$��0 0 0 10q�56      �� 57      
 �� Orientation$
 
Horizontal=�6   .+�58      �� $	��200��6   ��59      �� *NouvelleExtension+ �� Mode$ TwoWay=��� 5:      $��Ajouter��6   ���#4A90E2
�������$��80��5>      $��5q�5?      ��5;      �� *AjouterExtensionCommand+5?      5@      �� 5B      $	��100��5D      $	��250��5E      $��0 5 0 0q�5F      $
��Left=�5G      ��5B      �� *ExtensionsCryptees+ �� SelectedItem 5C      �� *ExtensionSelectionnee+$ TwoWay=�5G      �� 5H      $��	Supprimer��6   ���#D9534F
�������$	��100��5L      $
��Left=�5M      $��80 5 5 5q�5N      ��5I      �� *SupprimerExtensionCommand+5N      �� 5P      $$��Détection de Logiciel Métier��6   $��20(�5Q      $
��Bold"�5R      
�������$��	10 0 0 10q�5T      �� 5U      $
 
Horizontal=�6   .+�5V      �� $	��250��6    �� 
IsReadOnly$
 True��5X      ��5W      �� *LogicielMetier+$ TwoWay=�5X      �� 5Y      $��	Parcourir��6   ���#4A90E2
�������$��80��5]      $��5q�5^      ��5Z      �� *#!SelectionnerLogicielMetierCommand+5^      5_      �� 5a      $��Langue��6   $��20(�5b      $
��Bold"�5c      
�������$��	10 0 0 10q�5e      �� 5f      $	��150��5h      ��5f      �� *	Langues+ 5g      �� *LangueSelectionnee+5h      �� 5j      $!��Sauvegarder les paramètres��6   ���#4A90E2
�������$��40��5n      $��10q�5o      ��5k      �� *SauvegarderCommand+5o      �� 5q      $��Extensions Prioritaires��6   $��20(�5r      $
��Bold"�5s      
�������$��
10 20 0 10q�5u      �� 5v      $
 
Horizontal=�6   .+�5w      �� $	��200��6   ��6*   �� *NouvellePriorite+$ TwoWay=��� 5x      $��Ajouter��6   ���#4A90E2
�������$��80��5y   E   $��5q�6P   ��5x   /   �� *AjouterPrioriteCommand+5y   P   5z      �� 5{      $	��100��5}      $	��250��6'   $��0 5 0 0q�63   $
��Left=�6D   ��5{      �� *	Priorites+ 5|      �� *PrioriteSelectionnee+$ TwoWay=�5}   D   �� 5~      $��	Supprimer��6   ���#D9534F
�������$	��100��5   A   $
��Left=�5�      $��80 5 5 5q�64   ��5~   -   �� *SupprimerPrioriteCommand+5�   4   5�      5�      5�      5�      