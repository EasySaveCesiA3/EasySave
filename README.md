# EasySave

[![Downloads](https://img.shields.io/github/downloads/EasySaveCesiA3/EasySave/total.svg)](https://github.com/EasySaveCesiA3/EasySave/releases)
[![Langue FR](https://img.shields.io/badge/langue-FR-blue.svg)](#)
[![Langue US](https://img.shields.io/badge/langue-US-blue.svg)](#)

**EasySave** est un logiciel professionnel de sauvegarde développé par **ProSoft**. Il permet de sauvegarder automatiquement vos données de manière sûre et efficace. Son objectif : offrir une solution adaptée aux besoins des entreprises et des particuliers, avec une prise en main simplifiée et un niveau de sécurité élevé.

## Sommaire

1. [Contexte](#contexte)  
2. [Caractéristiques](#caractéristiques)  
3. [Gestion des versions](#gestion-des-versions)  
4. [Installation](#installation)  
5. [Utilisation](#utilisation)  
6. [Documentation et support](#documentation-et-support)  
7. [Contributions](#contributions)  
8. [Licence](#licence)  
9. [Équipe et remerciements](#équipe-et-remerciements)

---

## Contexte

Dans le cadre de la suite logicielle **ProSoft**, EasySave répond à un besoin métier : **sécuriser et automatiser les sauvegardes de données**. Sous la responsabilité du DSI, notre équipe gère le développement et la maintenance de ce projet en C# (.NET 8.0). Nous veillons à :

- Fournir un logiciel stable et pérenne.
- Faciliter l’intégration dans un environnement professionnel.
- Respecter les bonnes pratiques de code (lisibilité, maintenabilité, limitation des duplications).
- Proposer une documentation claire pour l’utilisateur final et pour le support technique.

---

## Caractéristiques

- **Multilingue** : disponible en français et en anglais.
- **Modes de sauvegarde** :
  - Sauvegarde **complète** ou **incrémentale** (selon la version).
  - Sauvegarde **parallèle** (à partir de la v3.0) pour accélérer le traitement.
- **Gestion des logs** : en JSON ou XML, pour une traçabilité fine des opérations.
- **Intégration de cryptage externe** : utilisation du logiciel *CryptoSoft* (v1.1+).
- **Interface** :
  - **Console** (versions 1.x).
  - **Graphique (GUI)** à partir de la version 2.0.
- **Installation conviviale** : installeur Windows (Setup Wizard) avec chemin par défaut `C:\Program Files (x86)\EasySave`.
- **Compatibilité** : Windows 10 ou supérieur, .NET 8.0 requis.

---

## Gestion des versions

EasySave évolue selon des versions majeures et mineures, chacune apportant de nouvelles fonctionnalités ou des améliorations :

| Version | Fonctionnalités principales                                                      |
|---------|----------------------------------------------------------------------------------|
| **1.0** | - Interface console<br>- Logs JSON/XML<br>- Limitation à 5 sauvegardes mono-thread |
| **1.1** | - Intégration de *CryptoSoft* (cryptage externe)<br>- Choix du format de logs      |
| **2.0** | - Passage à une interface **graphique**<br>- Ajout d'informations (temps, statut) dans les logs |
| **3.0** | - Sauvegardes parallèles<br>- Gestion de fichiers **prioritaires**<br>- Supervision de la charge réseau |

Chaque publication est accompagnée d’un fichier **Release Notes** détaillant les nouveautés, correctifs et points d’attention.

---

## Installation

### Prérequis

- **Système d’exploitation** : Windows 10 ou supérieur  
- **Runtime** : .NET 8.0 ou version supérieure  
- **Espace disque** : ~100 Mo minimum  
- **Accès internet** : recommandé pour les mises à jour

### Étapes

1. **Télécharger** l’installeur (`EasySaveSetup.exe`) depuis la [page GitHub Releases](#).
2. **Lancer** l’installateur et suivre l’assistant :
   - Choisir la langue (Français / Anglais).
   - Sélectionner le dossier d’installation par défaut : `C:\Program Files (x86)\EasySave`.
3. **Cliquer** sur *Install* pour valider l’installation.
4. À la fin, l’assistant propose de **lancer EasySave** ou de fermer l’installateur.

*(Exemples d’écrans d’installation ci-dessous)*

![Wizard Step 1](https://via.placeholder.com/400x300?text=Setup+EasySave+v1.0)
![Wizard Step 2](https://via.placeholder.com/400x300?text=Installation+Folder)
![Wizard Step 3](https://via.placeholder.com/400x300?text=Ready+to+Install)

---

## Utilisation

1. **Démarrer** EasySave depuis le menu Démarrer (Windows) ou via l’icône sur le Bureau.
2. **Sélectionner** les dossiers ou fichiers à sauvegarder.
3. **Choisir** le type de sauvegarde :
   - **Complète** : copie intégrale des fichiers.
   - **Incrémentale** : copie uniquement des fichiers modifiés (v1.x).
   - **Parallèle** (v3.0) : plusieurs sauvegardes simultanées.
4. **Consulter** les logs (JSON/XML) pour vérifier le statut et les performances.

## Support

- **Support technique** :  
  - Emplacement par défaut : `C:\Program Files (x86)\EasySave`  
  - Fichiers de configuration : `C:\ProgramData\EasySave\Config`  
  - Configuration minimale : Windows 10, .NET 8.0, 100 Mo d’espace disque
- **Diagrammes UML** : réalisés avec **Mermaid**
- **FAQ / Issues** : en cas de problème, [ouvrez une issue](#) sur GitHub ou contactez l’équipe support à `support@prosoft.com`.

---

## Contributions

Pour contribuer à EasySave :

1. **Forkez** ce dépôt GitHub.
2. **Créez** une branche pour votre fonctionnalité ou correction :
   ```bash
   git checkout -b feature/ma-fonctionnalite
   ```
3. **Commitez vos modifications** :
   ```bash
   git commit -m "Ajout d'une nouvelle fonctionnalité"
   ```
4. **Poussez votre branche** :
   ```bash
   git push origin feature/ma-fonctionnalite
   ```
5. **Ouvrez une Pull Request** pour soumettre vos changements.

> **Bonnes pratiques** :  
> - Respecter les conventions de nommage C# / .NET.  
> - Éviter les duplications de code (pas de copier-coller).  
> - Rédiger les commentaires et la documentation en anglais (pour les filiales anglophones).  
> - Privilégier des méthodes/fonctions courtes pour une meilleure lisibilité.

---

## Licence

EasySave est distribué sous licence [MIT](LICENSE). Vous êtes libre de l’utiliser, de le modifier et de le redistribuer, sous réserve de respecter les conditions de la licence.

---

## Équipe et remerciements

- **Équipe ProSoft – Projet EasySave**
- **Contributeurs** : [Liste des contributeurs sur GitHub](#)

> Un grand merci à tous les testeurs et collaborateurs qui participent à l’amélioration continue d’EasySave !

---

**Dernière mise à jour :** *27/02/2025*
