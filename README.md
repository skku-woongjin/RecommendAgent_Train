# Recommend Agent Demo Project

> 장소 추천 모델(Recommend Agent)을 훈련할 수 있는 유니티 프로젝트입니다. <br/>

## 모델 설명 

<br />

## Prerequisite

`Unity` (version: 2021.3.6f1)
<br />
`Python` (version: 3.8.13)
<br />

## Getting Started


### Clone Repository

```shell script
$ git clone https://github.com/skku-woongjin/RecommendAgent_Train.git
$ cd RecommendAgent_Train
```
### Install ml-agents Package
```shell script
$ pip install mlagents==0.27.0
```

### How to Train

1.  Unity 프로젝트를 실행 OS에 맞춰 빌드합니다. 
2.  아래의 shell script를 실행합니다. 
```shell script
mlagents-learn [yaml파일 경로] --no-graphics --env=[빌드 파일 경로] --run-id=[run id] --num-envs=[동시에 실행할 환경 개수]
```
getmean_tri.onnx는 다음 shell script로 훈련하였습니다.  
```shell script
mlagents-learn Recommend.yaml --no-graphics --env=LinuxMono_Getmean/RecEnv  --run-id=dist_count_log --num-envs=32 
```
3. 훈련을 마치면, 모델이 저장된 경로가 다음과 같이 나타납니다. 
```shell script
[INFO] Exported [모델 경로]
```
- [ml-agents github](https://github.com/Unity-Technologies/ml-agents)에 훈련에 관한 더 많은 정보가 있습니다. 


## 파일 구조

```
.
├── README.md
├── Assets
│   ├── Demo
│   │   ├── Materials
│   │   ├── Models
│   │   │   ├── Junwon.onnx
│   │   │   └── getmean_tri.onnx
│   │   ├── Prefabs
│   │   │   ├── Dest.prefab
│   │   │   ├── Slider.prefab
│   │   │   ├── TrailPoint.prefab
│   │   │   ├── dot_agent.prefab
│   │   │   ├── dot_answer.prefab
│   │   │   ├── dot_userlog.prefab
│   │   │   └── user.prefab
│   │   ├── Scripts
│   │   │   ├── ClickDetector.cs
│   │   │   ├── Flag.cs
│   │   │   ├── GameManager.cs
│   │   │   ├── JunwonAgent.cs
│   │   │   ├── ModelOverrider.cs
│   │   │   ├── RecommendAgent.cs
│   │   │   ├── TrailEnergyDecrease.cs
│   │   │   ├── TrailGenerator.cs
│   │   │   └── TrailPoint.cs
│   │   └── TrainScene
│   │       └── NavMesh.asset
│   ├── ML-Agents
│   │   └── Timers
│   └── Plugins
│       ├── Borodar
│       │   └── RainbowHierachy
│       └── TextMesh Pro
├── Packages
└── ProjectSettings
```

<br />

- [Assets/Demo](링크) : Demo Scene 을 구성하는 Asset 모음
- [Assets/Demo/Materials](링크) : 3D 오브젝트에 씌울 Material 모음
- [Assets/Demo/Models](링크) : 훈련된 모델 모음
    - Junwon.onnx: 비교 모델, #visit, duration, distance 가 모두 높은 장소 추천
    - getmean_tri.onnx: [RecommendAgent_Train]()을 통해 훈련된 모델
- [Assets/Demo/Scripts](링크) : C# 스크립트 모음

## Components

- **[ClickDetector.cs]()**
  - 장소 클릭 시 유저가 해당 장소로 이동하도록 함
  <br />
- **[Flag.cs]()**
  - 장소 class 정의
  <br />
- **[GameManager.cs]()** 
  - 장소 특성 저장, 장소 방문 처리
  - UI 업데이트
  - 장소 배치, 초기화 등
  <br />
- **[JunwonAgent.cs]()** 
  - Junwon.onnx의 input과 output 처리
  <br />
- **[RecommendAgent.cs]()** 
  - getmean_tri.onnx의 input과 output 처리 
  <br />
- **[TrailEnergyDecrease.cs](),[TrailGenerator.cs](),[TrailPoint.cs]()** 
  - 유저 이동 시 나타나는 발자취 처리 
  <br />
    
