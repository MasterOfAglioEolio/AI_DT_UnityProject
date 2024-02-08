# AI_Project_Digital_Twin_With_Posco

Posco AI/BIGDATA ACADEMY AI PROJECT

Unity기반 디지털 트윈과 강화학습으로 가상물류 창고를 통해 물류 최적화를 구현하는 프로젝트입니다. 
![binpacking](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/673c5a41-b754-4e68-9017-71018be248a4)


# 프로젝트 목표


프로젝트의 주요 목표는 강화학습과 디지털 트윈을 통해 가상 물류창고에서의 물류최적화를 구현하는 것입니다.
먼저, 프로젝트 추진 방향은 아래와 같다.

1) 공간효율 극대화 상자 배치
: 강화학습을 통해 상자 면적을 고려해 팔레트 위 적재 방법을 도출한다.

2) 로봇팔 pick and place
: 최적 위치에 상자 배치 자동화를 구현한다.

3) 물류 창고내 적하장 구현
: 적하장에서 물류창고 내 운송 및 적재 프로세스 구현

![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/94c0e348-39fc-4930-a204-b7a5e6ed6e2f)


# 개발 환경

<h5> 해당 프로젝트는 "Unity-Robotics-Hub"를 참고하여 개발했습니다.</h5>
- [Unity Robotics Hub](https://github.com/Unity-Technologies/Unity-Robotics-Hub)

## Pick and Place 구현 
- Unity Community(C#)
- ROS 2

## 강화 학습 구현
### 기술 세부
- tensorflow
- keras
- gym
- numpy
- sb3_contrib

# 실행 방법

### Unity & ROS SetUp
- [해당 링크 참고](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/pick_and_place/README.md)

### dqn을 돌리려면

- binpacking_gym 내부에 run_model_v0.py(binpacking_posco_v0, v4), run_model_v1.py(binpacking_posco_v1, v2, v3)를 돌리면 가능

### ppo를 돌리려면

- binpacking_gym 내부에 stable_baseline3.py(binpacking_poscoppo_v1) 돌리면 가능

### maskableppo를 돌리려면

- binpacking_gym 내부에 maskable_ppo.py(binpacking_posco_v00) 돌리면 가능

### maskable_ppo의 결과를 뽑아내려면

- maskppo_predict.py(binpacking_poscopredict-v00)를 돌린다.


# 로봇팔 Pick And Place 
## Process
![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/402807b6-648a-43e6-aa59-1cdae1212e42)


## Enviroment
![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/db15ad83-7448-4c05-b341-d55c09fbb5b8)

