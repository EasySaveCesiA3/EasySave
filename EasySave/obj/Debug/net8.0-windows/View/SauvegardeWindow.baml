   M S B A M L   `   `   `  ����F  BEasySaveApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ViewModels
ViewModels  5   	       Views.SauvegardeWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'vmclr-namespace:ViewModels     5   	     � Title$  Démarrer Sauvegarde��5   	   $	��590��6%   $	��500��62    � WindowStartupLocation$ CenterScreen=�5	   	   
������  � AllowsTransparency$ False��5   	    � WindowStyle$
 None=�5   	    � DataContext 5         ViewModels.SaveViewModel  5   
   .��5      �� ���#2E2E2E �� CornerRadius$ 10��6"    �� Padding$ 10q�64   ���#444$��2q�6T   ._�5   
   �  � RowDefinitions 5      �� 5      $��40 �6    �� 5      $��* �6    .+�5      � -   5        $��0��5      $��40��6    ���#444.+�5      �� $��Démarrer une Sauvegarde��6   
�������$��Center=�5      $
��Left=�67   $��10q�5      $��16(�6(   $
��Bold"�66   �� 5      $��X��6   $��30��6%   $��30��60   $��Right=�6<   $��0,5,5,0q�6X   
������ 
�������$��0q�5    E   $
��Bold"�5!      $��14(�6+   ��5"      �� *CloseWindowCommand+��5#      ��  �� RelativeSource �� 	 �� AncestorType
	 Window5$      �� 5'      $��1��6   $��Center=�6&   $��Center=�6C   $��0,10,0,0q�6^   .+�5)      �� $��Nom de la sauvegarde :��6   $��16(�6:   
�������$��10q�6[   �� 5*      $	��300��5+      $��30��6&   ���#3A3A3A
�������
�������$��1q�5,   U   ��5*      �� *NomSauvegarde+
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5,   U   �� 5.      $#��Chemin de sauvegarde source :��6   $��16(�6A   
�������$��10q�6b   �� 5/      $	��300��50      $��30��6&   ���#3A3A3A
�������
�������$��1q�51   U   ��5/      �� *CheminSauvegardeSource+$
 TwoWay=�$ PropertyChanged=�51   U   �� 53      $%��Sélectionner le dossier source��6   $	��300��55      $��40��6%   $��10q�61   ���#4A90E2
�������$
��Bold"�56   A   ��54      �� *SelectionnerDossierCommand+56   A   �� 58      $"��Chemin de sauvegarde cible :��6   $��16(�6@   
�������$��10q�6a   �� 59      $	��300��5:      $��30��6&   ���#3A3A3A
�������
�������$��1q�5;   U   ��59      �� *CheminSauvegardeCible+$
 TwoWay=�$ PropertyChanged=�5;   U   �� 5=      $$��Sélectionner le dossier cible��6   $	��300��5?      $��40��6%   $��10q�61   ���#4A90E2
�������$
��Bold"�5@   A   ��5>      �� *!SelectionnerDossierCibleCommand+5@   A   �� 5B      $��Type de sauvegarde :��6   $��16(�68   
�������$��10q�6Y   �� 5C      $	��300��5E      $��30��6'   ���#3A3A3A
���   �
�������$��1q�5F   V    �� 
IsEditable$ False��5G      ��5C      �� *TypesSauvegarde+ �� SelectedItem 5D      �� *TypeSauvegarde+$
 TwoWay=�$ PropertyChanged=�5G      �� 5J      $��Crypter les fichiers��6   $	��200��5K      $��40��6%   $��10q�61   ���#4A90E2
�������$
��Bold"�5L   A   ��5J   8   �� *CrypterFichiersCommand+5L   A   �� 5N      $��Lancer la sauvegarde��6   $	��200��5O      $��40��6%   $��10q�61   ���#4A90E2
�������$
��Bold"�5P   A   $��0q�6S   ��5N   8   �� *LancerSauvegardeCommand+5P   S   5Q      5R      5S      5T      