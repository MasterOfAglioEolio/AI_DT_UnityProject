# AI_Project_Digital_Twin_With_Posco

Posco AI/BIGDATA ACADEMY AI PROJECT

Unity기반 디지털 트윈과 강화학습으로 가상물류 창고를 통해 물류 최적화를 구현하는 프로젝트입니다. </br>


![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/b75646a6-addf-4af9-b7c8-9e4ae0544200)



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

### Asset
- (Factory Interior + Factory Props Vol 1 - BUNDLE ($69.99))[https://assetstore.unity.com/packages/3d/props/factory-interior-factory-props-vol-1-bundle-229757]
(저작권 문제로 해당 repository에는 commit하지 않았습니다.)

## 강화 학습 구현
- Python3

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

# 강화학습
## Customize-Enviroment
Curriculum learning 은 인간이 학습하는 프로세스를 모방하여, 쉬운 난이도의 데이터를 먼저
학습하고 점차 어려운 데이터를 학습하는 학습전략을 채택하여 모델의 학습 수렴 속도와
성능에서 성과를 보이는 학습 전략을 말한다.</br>
본 프로젝트는 큰 action space 와 직사각형 규격의 박스를 적재한다는 상황의 복잡성과 더불어
매우 높은 성능의 모델을 만드는 것을 목표로 하였다. 때문에 curriculum learning 을 통해 쉬운
단계의 모델을 학습하는 것으로 시작하여, 해당 모델을 순차적으로 상속받아 최종적으로
복잡하고 무거운 모델의 성능을 최대화하였다.

![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/7aeb9004-35af-4de5-ae84-14e4dde2c3c4)

최종적으로는 curriculum v0 으로 진행한다. 정사각형 박스를 순서에 맞게 받아와 훈련에 사용하였다. pallete의 사이즈는 10x10
으로 구성되어있다. threshold(에피소드 종료 기준값)는 0.8로 설정하였다.

## 강화학습 동작
![new_boxsequence](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/a76b1e49-677a-4d34-8d07-3739fb32b347)

강화학습 모델 학습 결과로 박스의 Action, 즉 그리드 위치와 박스의 크기를 담은 Json 파일이 작성되며,
이 데이터를 Unity 에서 읽어들여 시각적으로 bin packing 이 구현될 수 있게 하였다.

# 로봇팔 Pick And Place 
## Pick And Place 고도화
![기존모델](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/f4a7f570-7896-4173-b39e-30a36d9e35bf)|![ezgif com-gif-maker](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/56c2d5aa-5d42-4e8c-94e0-b12de7bb36a9)
|---|---|
|기존 모델(1 Pick 1 Place)|고도화 모델(N Pick N Plce)|
## Process
![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/402807b6-648a-43e6-aa59-1cdae1212e42)


## Enviroment
![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/db15ad83-7448-4c05-b341-d55c09fbb5b8)

1.(컨베이어 벨트) 
- 고정위치의 로봇 팔에 택배 상자를 공급하기 위한 컨베이어 벨트를 구현하기 위해, ‘Remake-BeltConveyor-System’영상을 참고해 직접 제작하였다. 물품이 컨베이어 벨트 끝단에서 생성되며,
컨베이어 벨트 1 칸에는 물품 1 개가 배치되도록 일정한 간격으로 물품이 이동하도록 구현했다.
특히, 로봇 팔이 컨베이어 벨트 위 물품을 잡기 위해서는 정해진 위치에서 물품이 정지되어야 한다.
하지만 해당 영상에서 소개하는 컨베이어 벨트는 정지 기능이 없다. 그래서 물품이 picking
지점까지 이동시킨 후 picking 될 때까지 정지하는 기능을 Raycast 함수를 통해 물체를 인식하도록
함으로써 구현하였다. 

2.(로봇팔) 
- 이번 프로젝트에서 사용할 로봇 팔은 “Niryo”사의 “Niryo One”을 사용하였고, 6 자유도를 가지고 있다. 해당 로봇
팔을 채택한 이유는 교육용으로 제작되어, 가상환경에서 로봇 팔을 구현 시켜주는 URDF 파일이
배포되어 있다는 점이다.
[링크](https://github.com/Unity-Technologies/Unity-Robotics-Hub)
위 주소는 로봇 팔의 Pick and Place 를 구현하기 위해서 활용한 자료이다. 해당 자료는 unity
환경에서 ROS 와 연동하여 로봇 팔을 제어하는 방법을 소개하는 튜토리얼로, 실제 로봇 팔에서
최종적으로 Pick and Place 를 구현하는 것까지 소개한다. 하지만, 해당 튜토리얼은 한 물체와 한
목적지(즉, 1 pick 1 place) 에 대한 실행 방법을 알려준다는 점에서 우리가 추구하는 방식과 달랐다.
그래서 우리는 프로젝트에 맞게 기존 튜토리얼의 1 pick 1 place 방식에서 N pick N place 로
수정하였다.

3.(팔레트&박스) 
- 팔레트는 Pick and Place 과정에서 물품이 적재될 공간이자 좌표의 역할을 한다.
팔레트의 모델링은 유니티 Asset store 에서 Asset 을 다운받아 사용했다. 강화학습의 결과로
전달되는 10*10 그리드 좌표를 기준으로 접근하기 위해 팔레트의 월드 좌표를 기준으로 좌표에
접근하기 위해 박스의 크기와 그리드의 크기를 고려하여 월드 좌표를 계산하여 변환된 그리드
좌표에 맞추어 Target Box 를 적재한다.

- 상자는 Pick and Place 에서 Target 의 역할을 수행한다. 박스 형의 object 를 생성 후, box material
texturer 을 씌어 생성했다. 박스 종류는 3 가지이며 우체국의 박스 1~3 호 규격을 참고함으로써
각각 10x10, 20x20, 33x30 이다.

## 최종 동작
![new_boxsequence](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/a76b1e49-677a-4d34-8d07-3739fb32b347)|![binpacking](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/673c5a41-b754-4e68-9017-71018be248a4)
|---|---|
|강화 학습 결과로 그리드 위치, Box Size Json 파일 생성|Json 파일 기반으로 Box Bin Packing| 




## 

