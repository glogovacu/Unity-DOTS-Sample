# Unity DOTS Sample
## Overview
Lets imagine a battefield of thousands of objects clashing how will the performace differ using the MonoBehaviour and DOTS implementation. Lets start simple, this is going to be the gameplay loop.

![image](https://github.com/glogovacu/Unity-DOTS-Sample/assets/125755319/dbeb690f-e869-4fdf-b5d2-8860d5b3fb7e)

### MonoBevaiour Mono Build
How will Unity MonoBehaviour perform if we have 2000 soldiers (capsules) clashing with each other.

![image7](https://github.com/glogovacu/Unity-DOTS-Sample/assets/125755319/3b034142-ba48-4c8c-9dcb-ce372f2a37f1)
The results are not the best we get 7 FPS. 

### DOTS IL2CPP Build
What happens if we use DOTS implementation with IL2CPP?

![image2](https://github.com/glogovacu/Unity-DOTS-Sample/assets/125755319/a2412b99-3c4a-48f0-a31c-a7fee58b8e3e)

Pheominal results with 3500% increase.

## Analytics
I have conducted tests for different amount of entities for each of the build, including or excluding ECS, Job or Burst. Here are the results:

![MonoChart ENG](https://github.com/glogovacu/Unity-DOTS-Sample/assets/125755319/f2ca9c87-6ebc-465f-b61b-47e7b7cb830b)

![IL2CPP Chart ENG](https://github.com/glogovacu/Unity-DOTS-Sample/assets/125755319/0aed0ccd-0956-4fee-aba6-a58b10fc00ed)

## Disclaimer 
If you attempt to download and run the project directly from this repository, you will encounter numerous errors, as this repo is intended solely for providing code examples. For a functional version of the project, please download the release versions available in the releases section of the repository.

## Controls
    F1 - MonoBehaviour Scene
    F2 - ECS Only Scene
    F3 - ECS with Job System Scene

