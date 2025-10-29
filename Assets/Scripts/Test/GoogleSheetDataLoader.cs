using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class GoogleSheetDataLoader : EditorWindow
{

    // 1단계에서 복사한 구글 시트 CSV 게시 링크를 여기에 붙여넣으세요.
    private const string SHEET_URL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQdE4QVY8LqSnHYoKjbWSmByEGSldNuHYr0FCoacHePzbfRVtFT4MxMutiNiDY1-u8l70InjRcW2RQH/pub?output=csv";

    private const string csvString = "ID,Speaker,AnimName,EventName,EventParam,NextID,Script,description\r\n2001001,사서,Library_ch_sit_idle,PlayAnim,null,2001002,null,* 플레이어가 도서관 중앙 위치(1)에 올 때까지 play\r\n2001002,사서,Library_ch_surprise,PlayAnim,null,2001003,null,* 플레이어가 (1)위치에 오면 play / 한 번 재생 후 2001003 이동\r\n2001003,사서,Library_ch_standup,PlayAnim,null,2001004,null,\r\n2001004,사서,Library_ch_walk,PlayAnim,null,2001005,null,* 플레이어 앞까지 walk \r\n2001005,사서,Library_ch_idle,ShowText,null,2001006,{fade} 오 새로운 손님이군!,\r\n2001006,사서,Library_ch_idle,ShowText,null,2001007,{fade} 반갑네!,\r\n2001007,사서,Library_ch_idle,CameraZoomIn,\"1.2, 0.8, middle_2\",2001008,{fade}모두가 행복해질 수 있고,\r\n2001008,사서,Library_ch_idle,CameraZoomIn,\"1.4 ,0.8, middle_2\",2001009,{fade}모두가 즐거울 수 있는!,\r\n2001009,사서,Library_ch_hug,CameraZoomOut,\"1.0, 0.8\",2001010,{horiexp} <bounce a=0.2>‘이세계 도서관’</bounce>에 온 것을 환영한다네!,마지막 모션 고정\r\n2001010,사서,Library_ch_idle,PlayAnim,null,2001011,null,\r\n2001011,System,null,WaitTimer,2,2001012,null,\r\n2001012,사서,Library_ch_idle,ShowText,null,2001013,\"{fade}그래, 이곳에 찾아온 당신은 지금 행복한가? 아니면 불행한가?\",*\r\n2001013,사서,Library_ch_idle,ShowText,null,2001014,{fade}그대의 감정이 어떤지 듣고 싶군.,*\r\n2001014,System,null,WaitForInput,BR_HAPPINESS,null,null,* 텍스트 입력 대기 / 입력 후 BR_HAPPINESS 분기로 이동/분기 종료 후 공통 스크립트 2001021로 이동\r\n2001015,사서,Library_ch_smile,ShowText,null,2001016,\"{fade}지금 행복하다니, 다행이군.\",\r\n2001016,사서,Library_ch_idle,ShowText,null,2001021,{fade}하지만 <waitfor=0.5>.<waitfor=0.5>.<waitfor=0.5>. 더 행복해질 수 있는 방법이 있다네!!,\r\n2001017,사서,Library_ch_idle,ShowText,null,2001018,{fade}음 그렇군.,\r\n2001018,사서,Library_ch_idle,ShowText,null,2001021,{fade}그렇다면 이곳을 한 번 체험해보는 건 어떤가?,\r\n2001019,사서,Library_ch_idle,ShowText,null,2001020,{fade}저런… 그리 행복하지 않은가보군!,\r\n2001020,사서,Library_ch_idle,ShowText,null,2001021,{fade}그렇다면 이곳을 잘 찾아왔네!!,\r\n2001021,사서,Library_ch_hug,ShowText,null,2001022,{horiexp}이곳은 <bounce a=0.2>‘이세계 도서관’!!</>,마지막 모션 고정\r\n2001022,사서,Library_ch_idle,ShowText,null,2001023,{fade}<b>이 세계에</b> 지친 사람들을 <b>이세계로</b> 초대하는 곳이지.,\r\n2001023,사서,Library_ch_idle,ShowText,null,2001024,{fade}이세계 안에서 당신은 무조건 행복해질 수 있을거야.,\r\n2001024,사서,Library_ch_idle,ShowText,null,2001025,{fade}여기 꽂혀있는 책들 중 한 권을 꺼내 읽으면 그대는 이세계 안의 인물이 되어 그의 삶을 살아간다네.,해당 대사 나올 때 4개의 책에 다 마우스 표시 뜨게 하기\r\n2001025,사서,Library_ch_smile,ShowText,null,2001026,{fade}어떤가? 상상만으로도 막 기대가 되지 않는가?,마우스 표시 사라지게 하기\r\n2001026,사서,Library_ch_idle,ShowText,null,2001027,{fade}준비가 됐다면 저기 왼쪽 위의 책을 골라서 시작해보게나.,이때 빈터발트 책에만 마우스 표시 뜨게하기 / 플레이어가 클릭할 때까지 계속 뜨게 하기\r\n2001027,사서,Library_ch_hug,CameraZoomIn,\"1.4 ,0.8, middle_2\",2001028,{horiexp}그럼 당신의 삶에 행복이 가득하길!,마지막 모션에서 고정\r\n2001028,사서,Library_ch_idle,CameraZoomOut,\"1.0, 0.8\",2001029,null,\r\n2001029,System,null,EndScript,null,null,null,*\r\n3001001,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001002,{fade}<wiggle> 꺄야야야아악!!! 공작님 아니세요?!</>,\r\n3001002,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001003,{fade}<wiggle> 어떡해!!!! 너무 잘생기셔서 내 눈이 멀 것 같아!!!</>,\r\n3001003,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001004,{fade}공작님 혹시 <b>약혼녀</b> 있으세요??? ,\r\n3001004,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001005,{fade}없으시다면 저희 중 한 명과 약혼을 맺으시는 건 어떠세요?,\r\n3001005,System,null,ShowChoice,CHOICE_3001005,null,null,\r\n3001006,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",CameraZoomIn,\"1.6, 0.8\",3001007,{fade}흠흠! 안녕하세요 공작님.,\r\n3001007,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001008,\"{fade}저는 첸트란트 남작가의 장녀, 클라리사 첸트란트입니다.\",\r\n3001008,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001009,{fade}저희 가문은 제국의 자금을 움직이는 손으로 불릴 정도로,\r\n3001009,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001010,{fade}왕실부터 상단까지 모두 저희에게 손을 벌리지요.,\r\n3001010,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001011,\"{fade}그러니, 저와 혼인하신다면 평생 돈 걱정하실 필요가 없답니다.\",\r\n3001011,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001012,\"{fade}어때요, 공작가의 금고… 제가 채워드릴까요?\",\r\n3001012,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001013,{fade}안녕하세요!!!!!!!!!!,\r\n3001013,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001014,{fade}소문으로만 듣던 공작님을 뵙게 되어 너무 영광입니다!!!!!,\r\n3001014,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001015,{fade}역시!!!!! 실물이 더 잘생기셨어요!!!,\r\n3001015,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001016,{fade}아참!!! 소개를 까먹었네요.,\r\n3001016,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001017,{fade}루미네 백작가의 차녀 리엘 루미네입니다!,\r\n3001017,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001018,{fade}저희 가문은 대대로 신성력을 타고나 치유에 능하답니다!,\r\n3001018,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001019,{fade}공작님께서는 전장의 선두로 나가실 때가 많으시죠?,\r\n3001019,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001020,{fade}가끔 다치시면 어떡하나 정말 마음을 졸인답니다.,\r\n3001020,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001021,{fade}그러니 제가 공작님 가까이에서 보필하며 치유할 수 있게 해주시겠어요?!,\r\n3001021,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001022,{fade}안…안녕하세요…,\r\n3001022,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001023,{fade}하인리히 공작가의 장녀 엘렌 하인리히입니다…,\r\n3001023,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001024,{fade}저희 가문은 대대로 마법을 연구하며,\r\n3001024,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001025,{fade}제국의 마도기관의 대부분을… 저희가 설계했어요…,\r\n3001025,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001026,{fade}원…원하시는 모든 기술들은… 저희 가문이… 다 만…만들어드릴 수 있어요…,\r\n3001026,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001027,{fade}그…그리고…사…사…사…,\r\n3001027,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001028,{fade}사…ㄹ…,\r\n3001028,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001029,{fade}아…아무것도 아닙니다!!!,\r\n3001029,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001030,{fade}그냥 잊어주세요!!!,\r\n3001030,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001031,\"{fade}자 그럼, 저희 중 누구와 약혼하실건가요?\",\r\n3001031,System,null,CameraZoomOut,\"1.0, 0.8\",3001032,null,\r\n3001032,System,null,WaitClick,Click_Lady,null,null,마우스 오버할 때 아웃라인 하얗게 빛나게 하기\r\n3001033,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",CameraZoomIn,\"1.6, 0.8\",3001034,{fade}후후… 현명한 결정이십니다. 공작님.,\r\n3001034,영애 2,\"Ladies_jump_2, Ladies_1, Ladies_3\",ShowText,null,3001043,{fade}후회하지 않으실 겁니다.,\r\n3001035,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",CameraZoomIn,\"1.6, 0.8\",3001036,\"{fade}진짜요?! 저, 선택된 거예요?!\",\r\n3001036,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001037,<wave>꺄아~~</>,\r\n3001037,영애 3,\"Ladies_jump_3, Ladies_1, Ladies_2\",ShowText,null,3001043,\"{fade}공작님을 위해서라면, 제 신성력을 전부 써도 좋아요!!\",\r\n3001038,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",CameraZoomIn,\"1.6, 0.8\",3001039,{fade}저… 저를… 선택하셨다고요…?,\r\n3001039,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001040,{fade}그럼 그동안 연습했던 말이 있는데요…!,\r\n3001040,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001041,{fade}사…사…사…,\r\n3001041,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001042,{fade}사랑님!! 공작합니다!!!!!,\r\n3001042,영애 1,\"Ladies_jump_1, Ladies_2, Ladies_3\",ShowText,null,3001043,{fade}으아악 취소!!!! 방금 건 실수예요!!!!,\r\n3001043,System,null,CameraZoomOut,\"1.0, 0.8\",3001044,null,\r\n3001044,System,null,EndScript,null,null,null,*\r\n3001045,Poor_boy,Poor_boy_quiver,ShowText,null,3001046,{fade}저… 저… 안녕하세요 공작님…,\r\n3001046,Poor_boy,Poor_boy_quiver,ShowText,null,3001047,{fade}이…이건 제가 직접 만든… 선물인데…,\r\n3001047,Poor_boy,Poor_boy_hand,PlayAnim,null,3001048,null,\r\n3001048,Poor_boy,Poor_boy_hand,ShowText,null,3001049,{fade}혹시 받아주시겠어요…?,\r\n3001049,System,Poor_boy_hand,ShowChoice,CHOICE_3001049,null,null,\r\n3001050,Poor_boy,Poor_boy_quiver,ShowText,null,3001051,{fade}제 정성스러운 선물을 받아주셔서... 너무 기뻐요!!,\r\n3001051,Poor_boy,Poor_boy_quiver,ShowText,null,3001052,{fade}역시 제 우상인 공작님은... 참 다정하시군요!!,\r\n3001052,Poor_boy,Poor_boy_quiver,CameraZoomIn,\"1.2, 0.8, middle_2\",3001053,{fade}참고로 이 선물은...! ,\r\n3001053,Poor_boy,Poor_boy_quiver,CameraZoomIn,\"1.4, 0.8, middle_2\",3001054,{fade}제가 키우는 <b>루키의 똥</>으로 만든 거예요…!,\r\n3001054,Poor_boy,Poor_boy_quiver,ShowText,null,3001055,{fade}루키가 누구냐고요? 제 사랑스러운 강아지랍니다!!!,\r\n3001055,Poor_boy,Poor_boy_quiver,ShowText,null,3001060,{fade}어때요?? 정말 예쁘죠?!,\r\n3001056,Poor_boy,Poor_boy_quiver,ShowText,null,3001057,{fade}그쵸… 저같은 더러운 사람이 만든 물건은 안 받고 싶으시겠죠…,\r\n3001057,Poor_boy,Poor_boy_quiver,CameraZoomIn,\"1.2, 0.8, middle_2\",3001058,{fade}그렇지만 공작님이 제 우상이라는 건 변하지 않아요…!,\r\n3001058,Poor_boy,Poor_boy_quiver,CameraZoomIn,\"1.4, 0.8, middle_2\",3001059,{fade}너…너무너무 멋지세요…!!,\r\n3001059,Poor_boy,Poor_boy_quiver,ShowText,null,3001060,{fade}저도 언젠가 꼭 공작님처럼 훌륭한 어른이 될 거예요…!!,\r\n3001060,System,Poor_boy_quiver,CameraZoomOut,\"1.0, 0.8\",3001061,,\r\n3001061,System,null,EndScript,null,null,,\r\n3001062,System,Flower_Vendor_walk,SetInteraction,\"NPC_Flower, 3001063\",3001063,,\"Shift 상호작용 성공 시, 3001063로 이동\"\r\n3001063,꽃 상인,Flower_Vendor_idle,StopPatrol,NPC_Flower,3001064,,\"대화 시작 직전, 상인의 이동을 멈춤.\"\r\n3001064,꽃 상인,Flower_Vendor_idle,ShowText,null,3001065,<bounce a=0.2>꽃 사세요~!</>,\r\n3001065,꽃 상인,Flower_Vendor_idle,ShowText,null,3001066,{fade}안녕하세요 공작님! 오늘도 매우 잘생기셨네요!!,\r\n3001066,꽃 상인,Flower_Vendor_idle,ShowText,null,3001067,{fade}그런 공작님께 어울리는 꽃 한 송이!! 골라보지 않으시겠어요?,\r\n3001067,꽃 상인,Flower_Vendor_idle,ShowText,null,3001068,{fade}첫 번째 꽃은 엄청나게 예쁜 루드베키아예요! 지금이 가장 아름다울 때랍니다!,\r\n3001068,꽃 상인,Flower_Vendor_idle,ShowText,null,3001069,{fade}두 번째 꽃은 아직 덜 핀 꽃이에요. 하지만 곧! 필 거랍니다!,\r\n3001069,꽃 상인,Flower_Vendor_idle,ShowText,null,3001070,{fade}그럼 둘 중 어떤 꽃을 고르시겠어요?,\r\n3001070,System,Flower_Vendor_idle,WaitClick,Click_Flower,null,,\r\n3001071,꽃 상인,Flower_Vendor_give,PlayAnim,null,3001072,,역재생\r\n3001072,꽃 상인,Flower_Vendor_idle,ShowText,null,3001073,{fade}오! 보는 눈이 있으시네요 공작님!,\r\n3001073,꽃 상인,Flower_Vendor_idle,ShowText,null,3001074,{fade}지금 가장 아름답게 핀 루드베키아를 아름다운 공작님께 드릴게요.,\r\n3001074,꽃 상인,Flower_Vendor_idle,ShowText,null,3001083,{fade}이 꽃의 꽃말은 <b>‘영원한 행복’</>이랍니다!,\r\n3001075,꽃 상인,Flower_Vendor_idle,ShowText,null,3001076,{fade}받아가신 꽃처럼 영원히 행복하시길!,\r\n3001076,꽃 상인,Flower_Vendor_idle,ShowText,null,3001077,{fade}대부분은 바로 눈에 띄는 걸 집어가시는데…,\r\n3001077,꽃 상인,Flower_Vendor_idle,ShowText,null,3001078,{fade}공작님은 기다림을 아시는 분인가봐요,\r\n3001078,꽃 상인,Flower_Vendor_idle,ShowText,null,3001079,\"{fade}조금만 기다리면, 이 꽃도 아름답게 피어날 거예요!\",\r\n3001079,꽃 상인,Flower_Vendor_idle,ShowText,null,3001080,{fade}각자 피는 시기는 모두 다 다르지만,\r\n3001080,꽃 상인,Flower_Vendor_idle,ShowText,null,3001081,{fade}결국 모두가 기다리는 건 가장 늦게 피는 꽃 아니겠어요?,\r\n3001081,꽃 상인,Flower_Vendor_idle,ShowText,null,3001082,{fade}그러니 지금 당장 눈앞에 보이는 아름다움이 없더라도,\r\n3001082,꽃 상인,Flower_Vendor_idle,ShowText,null,3001083,{fade}언젠가 피어날 순간을 기대하며 기다려보아요.,\r\n3001083,System,null,EndScript,null,null,,\r\n3001084,화가,artist_stop,ShowText,null,3001085,{fade}어이쿠 공작님! 잠깐 5초만 가만히 계셔주시겠습니까?,\r\n3001085,화가,artist_stop,ShowText,null,3001086,{fade}제가 지금 걸작을 그리고 있거든요!,\r\n3001086,화가,artist_stop,ShowText,null,3001087,{fade}딱 5초만 움직이지 말고 기다려보세요!!,\r\n3001087,System,artist_drowing,CheckPlayerCondition,BR_Time_Still_5S,null,null,Branch Logic Table로 이동 / 5초간 멈춤 성공 or 실패에 따라 이동되는 ID 상이\r\n3001088,화가,artist_stop,ShowText,null,3001089,<wiggle>어엇 움직였다!!!!!</>,\r\n3001089,화가,artist_stop,ShowText,null,3001090,{fade}아참 죄송합니다 조각상이 움직인 줄 알았는데 공작님이셨군요,\r\n3001090,화가,artist_stop,ShowText,null,3001091,{fade}조각상이나 공작님이나 둘 다 눈부시게 잘생긴 건 똑같은데,\r\n3001091,화가,artist_stop,ShowText,null,3001098,{fade}그래도 역시 우리 공작님이 훨씬 더 눈부시네요 허허!,\r\n3001092,화가,artist_drowing,ShowText,null,3001093,{fade}휴 다 됐습니다!,\r\n3001093,화가,artist_stop,ShowText,null,3001094,{fade}역시 모델이 좋으니까 그림이 더 잘 그려지네요.,\r\n3001094,화가,artist_stop,ShowText,null,3001095,{fade}요즘 제 그림 실력이 퇴화한 줄 알았는데 오늘 작품을 보니 아닌 것 같군요 하하!,\r\n3001095,화가,artist_stop,ShowText,null,3001096,{fade}공작님껜 특별히 무료로! 제 작품을 드리겠습니다.,\r\n3001096,화가,artist_stop,ShowText,null,3001097,{fade}<wave>대대손손 보관해주세요오오옹~~</>,\r\n3001097,System,null,ShowUI,\"portrait, 5s\",3001098,null,\r\n3001098,System,null,EndScript,null,null,null,\r\n3001099,학자,Scholar_talk,ShowText,null,3001100,{fade}아 공작님! 그때 남기신 논문은 잘 확인했습니다!!,\r\n3001100,학자,Scholar_talk,ShowText,null,3001101,{fade}어떻게 그런 생각을 하시는지!! ,\r\n3001101,학자,Scholar_talk,ShowText,null,3001102,{fade}감탄밖에 나오지 않았답니다.,\r\n3001102,학자,Scholar_talk,ShowText,null,3001103,{fade}그 중 가장 인상깊었던 부분은...,\r\n3001103,학자,Scholar_mumble,CameraZoomIn,\"1.2, 0.8, middle_2\",3001104,\"{horiexp}마법 탄환의 궤적이 단순한 포물선이 아니라 공기 중 마력 입자와 원소의 상호 진동, 중력 장의 비선형 왜곡까지 동시에 고려해야 한다는 점과, 각 속성별 마력 주파수 조합에 따라 궤도와 효과가 극도로 민감하게 달라진다는 사실이었습니다!! 특히 불속성과 바람속성을 동시에 적용할 경우, 탄환의 안정성과 충돌 에너지 계산이 복합적으로 얽혀, 기존 공식으로는 예측이 거의 불가능하다는 점에서는 너무 감동이었다고요!\",*\r\n3001104,학자,Scholar_mumble,CameraZoomIn,\"1.4, 0.8, middle_2\",3001105,\"{horiexp}그리고 제가 이 논문을 바탕으로 관찰한 결과, 마력 주파수와 원소 상호작용의 공명 지점을 정밀하게 계산하면 탄환이 자동으로 최적 궤도를 따라가며 충돌 효과를 극대화할 수 있다는 결론을 얻었습니다.\",\r\n3001105,학자,Scholar_talk,CameraZoomIn,\"1.6, 0.8, middle_2\",3001106,\"{fade}그래서, 이 해석에 대해서는 어떻게 생각하시나요?\",\r\n3001106,System,null,CameraZoomOut,\"1.0, 0.8\",3001107,,\r\n3001107,System,null,WaitForInput,BR_DUMMY,null,null,플레이어에게 입력을 받고 BR_DUMMY 규칙에 따라 로 이동\r\n3001108,학자,Scholar_mumble,ShowText,null,3001109,{fade}오…! 그런가요?,\r\n3001109,학자,Scholar_mumble,ShowText,null,3001110,{fade}역시!! 미천한 제 뇌로는 이해하지 못하겠네요.,\r\n3001110,System,null,EndScript,null,null,null,\r\n3001110,남자 상인,merchant_M_talk,ShowText,null,3001111,{fade}이번에 그 소문 들었나? 공작님이 날개 달린 용을 토벌했다는 얘기!!,\r\n3001111,여자 상인,merchant_Ｆ_talk,ShowText,null,3001112,{fade}뭐라구요?! 그 거대한 용을 혼자서요?,\r\n3001112,남자 상인,merchant_M_talk,ShowText,null,3001113,{fade}그래! 그 장면을 직접 본 사람들의 말로는 하늘이 번쩍!하고 반으로 갈라지는 듯했다고 하더군!,\r\n3001113,여자 상인,merchant_Ｆ_talk,ShowText,null,3001114,{fade}정말 이 제국에 둘도 없는 영웅이셔!!!,\r\n3001114,남자 상인,merchant_M_talk,ShowText,null,3001115,{fade}하하! 공작님 이름만 들어도 마물들이 벌벌 떨겠네!,\r\n3001115,여자 상인,merchant_Ｆ_talk,ShowText,null,3001116,{fade}그 분 덕분에 이 영지가 얼마나 평화로운지!!,\r\n3001116,System,null,EndScript,null,null,null,";

	// 데이터를 저장할 경로
	private const string SAVE_PATH = "Assets/Resources/DB/DialogueData";

    [MenuItem("Loader/Import Dialogue Data")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSheetDataLoader>("Dialogue Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import dialogue data from Google Sheet", EditorStyles.boldLabel);
        if (GUILayout.Button("Import"))
        {
			ImportData();
        }
    }

    private static async void ImportData()
    {
        Debug.Log("Starting data import...");

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // 웹에서 CSV 데이터 다운로드
                string csvData = await client.GetStringAsync(SHEET_URL);

                // 데이터 파싱 및 ScriptableObject 생성
                CreateScriptableObjects(csvData);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Error fetching data: {e.Message}");
            }
        }
    }

	private static void ImportData2()
	{
		CreateScriptableObjects(csvString);
	}

	private static void CreateScriptableObjects(string csvData)
    {
        // 저장 경로가 없으면 생성
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }

        // CSV 데이터를 줄 단위로 나눔
        string[] lines = csvData.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 줄(헤더)은 건너뛰고 두 번째 줄부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = ParseCsvLine(lines[i]);

            // 데이터가 충분하지 않은 줄은 건너뜀
            if (values.Length < 8) continue;

            // ScriptableObject 인스턴스 생성
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();

            // 각 변수에 데이터 할당
            data.id = values[0];
            data.speaker = values[1];
            data.animName = values[2];
            data.eventName = values[3];
            data.eventParam = values[4];
            data.nextID = values[5];
            //data.nextFalseID = values[6];
            //data.script = values[7];
            data.script = values[6];
			data.descript = values[7];

			// 파일 이름은 고유한 ID로 지정.
			string assetPath = $"{SAVE_PATH}/{data.id}.asset";
            AssetDatabase.CreateAsset(data, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Data import complete! {lines.Length - 1} assets created at {SAVE_PATH}");
    }

    // 쉼표로 구분된 라인을 파싱하는 간단한 함수 (따옴표 안에 쉼표가 있는 경우 처리)
    private static string[] ParseCsvLine(string line)
    {
        var parts = new System.Collections.Generic.List<string>();
        var currentPart = new System.Text.StringBuilder();
        bool inQuotes = false;

        foreach (char c in line)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                parts.Add(currentPart.ToString());
                currentPart.Clear();
            }
            else
            {
                currentPart.Append(c);
            }
        }
        parts.Add(currentPart.ToString());
        return parts.ToArray();
    }
}
#endif