# AI_Project_Digital_Twin_With_Posco

Posco AI/BIGDATA ACADEMY AI PROJECT

이 프로젝트는 Unity 기반의 가상 물류 창고에서 강화학습을 통해 물류 최적화를 구현하는 것을 목표로 합니다. </br>
디지털 트윈을 활용하여 가상 물류 창고를 구축하고, 강화학습 알고리즘을 적용하여 물류 작업의 최적화를 달성합니다. </br>

![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/b75646a6-addf-4af9-b7c8-9e4ae0544200)



# 프로젝트 목표


프로젝트의 주요 목표는 강화학습과 디지털 트윈을 통해 가상 물류창고에서의 물류최적화를 구현하는 것입니다.
먼저, 프로젝트 추진 방향은 아래와 같습니다다.

1) 공간효율 극대화 상자 배치
: 강화학습을 통해 상자 면적을 고려해 팔레트 위 적재 방법을 도출한다.

2) 로봇팔 pick and place
: 최적 위치에 상자 배치 자동화를 구현한다.

3) 물류 창고내 적하장 구현
: 적하장에서 물류창고 내 운송 및 적재 프로세스 구현

![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/94c0e348-39fc-4930-a204-b7a5e6ed6e2f)


# 개발 환경

<h5> 해당 프로젝트는 "Unity-Robotics-Hub"를 참고하여 개발했습니다.</h5>

> [Unity-Robotics-Hub](https://github.com/Unity-Technologies/Unity-Robotics-Hub)

## Pick and Place 구현 
- Unity Community(C#)
- ROS 2

### Asset
- [Factory Interior + Factory Props Vol 1 - BUNDLE ($69.99)](https://assetstore.unity.com/packages/3d/props/factory-interior-factory-props-vol-1-bundle-229757)

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

## Unity

### Unity & ROS SetUp
- [해당 링크 참고](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/pick_and_place/README.md)

### Unity Project 실행
- 위 링크를 통해 URDF 및 Docker 실행 완료 후 AI_DT_Project.unity 실행 ! 

## 강화학습

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

> Curriculum learning 은 인간이 학습하는 프로세스를 모방하여, 쉬운 난이도의 데이터를 먼저
학습하고 점차 어려운 데이터를 학습하는 학습전략을 채택하여 모델의 학습 수렴 속도와
성능에서 성과를 보이는 학습 전략을 말한다.</br>
본 프로젝트는 큰 action space 와 직사각형 규격의 박스를 적재한다는 상황의 복잡성과 더불어
매우 높은 성능의 모델을 만드는 것을 목표로 하였다. 때문에 curriculum learning 을 통해 쉬운
단계의 모델을 학습하는 것으로 시작하여, 해당 모델을 순차적으로 상속받아 최종적으로
복잡하고 무거운 모델의 성능을 최대화하였다.

![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/7aeb9004-35af-4de5-ae84-14e4dde2c3c4)

## 알고리즘 비교
![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/365afc12-5696-4fe1-acfe-637520853087)


![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/1b70a3a7-9ccb-4877-a356-ad5fa394fe2c)

> 본 프로젝트에서는 크게 4 단계로 알고리즘을 비교하였다. 이는 더 복잡한 문제를 풀기에 적절한 알고리즘을 채택하기 위함이다.
그 중, 가장 두드러지는 차이를 보낸 모델은 기본 PPO 모델과 Mask PPO 모델, 그리고 Mask PPO 모델의
모델 성능 차이였으며, 획기적인 학습시간 단축을 가능하게 한 Multi Processing 을 적용한 MaskPPO
모델이었다.
이들의 성능 차이를 모델의 tensorboard 출력 결과 지표를 통해 비교하였다.

![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/a24462fa-9ca5-4273-8b37-be4d1e90a55f)

> tensorboard를 통해 위와 같이 모델 훈련 결과를 확인할 수 있다. 중점적으로 보아야할 지표로는 loss값
과 variance가 있다. loss 값들은 0에 가까워 질수록 좋고, variance는 설명력으로 1에 가까울수록 좋은 지
표이다. 또한 len의 경우 한 에피소드의 길이고, reward의 경우 보상받은 점수를 의미한다. 에피소드의 길
이가 짧아지고 보상받은 점수가 증가할수록 훈련이 잘 되고 있다는 것을 알 수 있다.

- 결과적으로 Mask PPO 모델을 활용한 V0 환경에서 강화학습을 진행한다.


## 강화학습 동작
![new_boxsequence](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/a76b1e49-677a-4d34-8d07-3739fb32b347)

강화학습 모델 학습 결과로 박스의 Action, 즉 그리드 위치와 박스의 크기를 담은 Json 파일이 작성되며,
이 데이터를 Unity 에서 읽어들여 시각적으로 bin packing 이 구현될 수 있게 하였다.

# 로봇팔 Pick And Place 

## Pick And Place 프로세스

1) Target Box 은 Conveyor belt 를 따라 picking 지점까지 이동
2) Robotic arm 이 이동된 Target Box 을 인식
3) Robotic arm 은 인식된 Target Box 의 좌표를 토대로 picking
4) Robotic arm 은 picking 한 Target Box 을 destination 까지 이동
5) 적재할 destination 에 도착했을 때 Robotic arm 은 Target Box 을 placing
위 과정을 반복하며 각각의 Box 들을 팔레트 위 특정 좌표에 적재 시키며 빈 공간 최소화


## Enviroment
![image](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/db15ad83-7448-4c05-b341-d55c09fbb5b8)

---

### 1.(컨베이어 벨트) 
- 고정위치의 로봇 팔에 택배 상자를 공급하기 위한 컨베이어 벨트를 구현하기 위해,[Remake-BeltConveyor-System](https://www.youtube.com/watch?time_continue=969&v=Zev8-i6uX_U&embeds_referring_euri=https%3A%2F%2Fwww.notion.so%2F&source_ve_path=MjM4NTE&feature=emb_title) 영상을 참고해 직접 제작하였다. 물품이 컨베이어 벨트 끝단에서 생성되며,
컨베이어 벨트 1 칸에는 물품 1 개가 배치되도록 일정한 간격으로 물품이 이동하도록 구현했다.
특히, 로봇 팔이 컨베이어 벨트 위 물품을 잡기 위해서는 정해진 위치에서 물품이 정지되어야 한다.
하지만 해당 영상에서 소개하는 컨베이어 벨트는 정지 기능이 없다. 그래서 물품이 picking
지점까지 이동시킨 후 picking 될 때까지 정지하는 기능을 Raycast 기능을 통해 물체를 인식하도록
함으로써 구현하였다.

![컨베이어](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/35e74497-f62f-469c-bab2-1944218360b0)

---

### 2.(로봇팔) 
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

### Pick And Place 고도화

<table>
  <tr>
    <td align="center">
      <img src="https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/f4a7f570-7896-4173-b39e-30a36d9e35bf" width=450 alt="기존모델">
    </td>
    <td align="center">
      <img src="https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/56c2d5aa-5d42-4e8c-94e0-b12de7bb36a9" width=450 alt="ezgif com-gif-maker">
    </td>
  </tr>
  <tr>
    <td align="center">기존 모델 (1 Pick 1 Place)</td>
    <td align="center">고도화 모델 (N Pick N Place)</td>
  </tr>
</table>

---

### 3.(팔레트&박스) 
- 팔레트는 Pick and Place 과정에서 물품이 적재될 공간이자 좌표의 역할을 한다.
팔레트의 모델링은 유니티 Asset store 에서 Asset 을 다운받아 사용했다. 강화학습의 결과로
전달되는 10*10 그리드 좌표를 기준으로 접근하기 위해 팔레트의 월드 좌표를 기준으로 좌표에
접근하기 위해 박스의 크기와 그리드의 크기를 고려하여 월드 좌표를 계산하여 변환된 그리드
좌표에 맞추어 Target Box 를 적재한다.

#### 그리드 좌표 > Unity 월드 좌표 변환 후 Box 배치
  
![KakaoTalk_20240211_182655021](https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/cd03ba06-7814-422d-83aa-44a76c3685a9)


- 상자는 Pick and Place 에서 Target 의 역할을 수행한다. 박스 형의 object 를 생성 후, box material
texturer 을 씌어 생성했다. 박스 종류는 3 가지이며 우체국의 박스 1~3 호 규격을 참고함으로써
각각 10x10, 20x20, 33x30 이다.

## 최종 동작

> [시연 영상 링크](https://youtu.be/BqrYe2p-a2w)
- 강화학습을 통해 나온 결과를 바탕으로 Unity 환경에서 물류 적재 최적화 시뮬레이션을 수행합니다.

<table>
  <tr>
    <td align="center">
      <img src="https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/a76b1e49-677a-4d34-8d07-3739fb32b347" width="450" alt="new_boxsequence">
    </td>
    <td align="center">
      <img src="https://github.com/XgitalBounce/AI_DT_UnityProject/assets/60294084/673c5a41-b754-4e68-9017-71018be248a4" width="450" alt="binpacking">
    </td>
  </tr>
  <tr>
    <td align="center">강화 학습 결과로 그리드 위치, Box Size Json 파일 생성</td>
    <td align="center">Json 파일 기반으로 Box Bin Packing</td>
  </tr>
</table>


## 개선 방향
1. ML-Agent를 활용한 동작 학습: 현재는 Python 과 Unity 간 좌표 제공 방식을 사용하여 강화학습을 구현하고 있다. 하지만 유니티 내 ML-Agent를 활용하여 로봇팔이 실제 동작을 학습할 수 있도록 구현하는 방식으로 개선 가능하다.
   
2. 실제 기업 데이터 적용: 아쉽게도 우리는 실제 기업 데이터를 구하지 못하여 최적화 프로그램에 적용시켜 보지 못했다. 하지만 기업 데이터를 활용하여 우리가 구현한 최적화 프로그램의 성능을 평가하고 개선할 수 있다면 더 좋은 결과를 얻을 수 있을 것이다.
   
3. 실시간 개체 인식 기반 적재 방식: 향후 개선 방향으로는 시나리오 방식의 bin packing이 아닌, 실시간으로 인식된 개체를 기반으로 최적의 적재 위치를 도출하는 방식으로 개선할 수 있다. 이를 통해 더 효율적인 적재가 가능하게 될 것이다.



