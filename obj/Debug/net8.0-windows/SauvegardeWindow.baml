   M S B A M L   `   `   `  ����  EasySaveApp"    EasySaveApp.SauvegardeWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Démarrer Sauvegarde��5   	   $	��400��6%   $	��500��62    � WindowStartupLocation$ CenterScreen=�5   	   
������  � AllowsTransparency$ False��5   	    � WindowStyle$
 None=�5   	   .��5
      �� ���#2E2E2E �� CornerRadius$ 10��6"    �� Padding$ 10q�64   ���#444$��2q�6T   ._�5   
   �  � RowDefinitions 5      �� 5      $��40 �6    �� 5      $��* �6    .+�5      � -   5        $��0��5      $��40��6    ���#444.+�5      �� $��Démarrer une Sauvegarde��6   
�������$��Center=�5      $
��Left=�67   $��10q�5      $��16(�6(   $
��Bold"�66   �� 5      -   5        $��X��5      $��30��6%   $��30��60   $��Right=�6<   $��0,5,5,0q�6X   
������ 
�������$��0q�5   E   $
��Bold"�5      $��14(�6+   5      5      �� 5      $��1��6   $��Center=�6&   $��Center=�6C   $��0,10,0,0q�6^   .+�5       �� $��Nom de la sauvegarde :��6   $��16(�6:   
�������$��10q�6[   �� 5!      -   5         ��Name$ NomSauvegardeTextBox��5!      $	��300��68   $��30��6D   ���#3A3A3A
�������
�������$��1q�5"   U   �� 5$      $��Chemin de sauvegarde :��6   $��16(�6:   
�������$��10q�6[   �� 5%      -   5        $ CheminSauvegardeTextBox��5%      $	��300��6;   $��30��6G   ���#3A3A3A
�������
�������$��1q�5&   U   �� 5(      -   5        $-��'Ouvrir l'explorateur pour sélectionner��5(      $	��300��5)      $��40��6%   $��10q�61   ���#4A90E2
�������$
��Bold"�5*   A   $��0q�6S   �� 5,      -   5        $��Crypter les fichiers��5,      $	��200��5-      $��40��6%   $��10q�61   ���#4A90E2
�������$
��Bold"�5.   A   �� 50      -   5        $��Lancer la sauvegarde��50      $	��200��6W   $��40��6c   $��10q�6o   ���#4A90E2
�������$
��Bold"�51   A   $��0q�6S   52      53      54      55      