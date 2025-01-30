# Biofeedback Awareness

This research presents the results on the impact of biofeedback awareness on the effectiveness of physiological interaction with Electrodermal Activity (EDA) as the primary metric within an adaptive Virtual Reality environement.

Unity Project was created using the Biosignalplux Unity sample in Unity 2019. It was then migrated to Unity 2020.3.5f1 and integrated the signal processing from the EDA Sensor with the 1-Channel OpenBan hub from biosignalplux (a).
The project is implemented to work with a HTC Vive Pro Eye (a). Scenes include a nature environment for the relaxing phase (b), a VR-adapted mental arithmetic task (c) for stress baseline measures, and an EDA-adaptive nature environment (d).

We provide the dataset, including the Unity project, the log files, and the scripts we used for the evaluation of the results. 
A detailed description of the study procedure and hardware integration can be found in the related dissertation (*doctoral thesis reference here, coming soon*).

You find the whole evaluation data for the muscle location study here: https://www.dropbox.com/home/Evaluation%20EMG%20Biofeedback%20Modality/Evaluation%20EMG%20REACTION 

We probed whether informing individuals of their capacity to manipulate the VR environment’s weather impacts their physiological stress responses  in a user study (N=30). Our results indicate lower EDA levels among participants who were informed of their biofeedback control than among those who were not informed. Interestingly, the participants who were informed about their ability to control the environment also manifested variations in their EDA responses. Participants who were not informed of their ability to control the weather showed decreased EDA measures until the end.

![Storm](https://github.com/user-attachments/assets/7e5f4a55-871a-4fb1-bd44-e6de122119d2)

```@inproceedings{10.1145/3613905.3650830,
author = {Sehrt, Jessica and Yilmaz, Ugur and Kosch, Thomas and Schwind, Valentin},
title = {Closing the Loop: The Effects of Biofeedback Awareness on Physiological Stress Response Using Electrodermal Activity in Virtual Reality},
year = {2024},
isbn = {9798400703317},
publisher = {Association for Computing Machinery},
address = {New York, NY, USA},
url = {https://doi.org/10.1145/3613905.3650830},
doi = {10.1145/3613905.3650830},
articleno = {76},
numpages = {7},
keywords = {Awareness, Biofeedback, Electrodermal Activity, Stress, Virtual Reality},
location = {Honolulu, HI, USA},
series = {CHI EA '24}
}
