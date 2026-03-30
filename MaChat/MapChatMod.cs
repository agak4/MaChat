using Godot;
using BaseLib; // BaseLib 의존성
using System;

namespace MapChat
{
    /// <summary>
    /// MapChat 모드의 메인 클래스입니다.
    /// 게임이 모드를 로드할 때 Initialize()가 호출됩니다.
    /// </summary>
    public class MapChatMod
    {
        // 모드 인스턴스 (싱글턴 패턴)
        public static MapChatMod? Instance { get; private set; }
        
        // 설정
        private ChatConfig _config = new ChatConfig();
        
        // UI 컴포넌트
        private ChatUI? _chatUI;
        private ChatDisplay? _chatDisplay;
        
        // 단축키 키코드
        private Key _hotkeyCode = Key.F6;

        /// <summary>
        /// 모드 초기화 - 게임 시작 시 한 번 호출됩니다.
        /// </summary>
        public void Initialize()
        {
            Instance = this;
            
            GD.Print("[MapChat] 모드 초기화 시작...");
            
            // 설정 로드
            _config = ChatConfig.Load();
            
            // 단축키 파싱
            ParseHotkey();
            
            // UI 생성
            SetupUI();
            
            // 입력 이벤트 등록
            RegisterInputHandler();
            
            GD.Print($"[MapChat] 초기화 완료! 단축키: {_config.Hotkey}");
        }

        /// <summary>
        /// 설정 파일의 단축키 문자열을 Godot Key로 변환합니다.
        /// </summary>
        private void ParseHotkey()
        {
            _hotkeyCode = _config.Hotkey.ToUpper() switch
            {
                "F1" => Key.F1,
                "F2" => Key.F2,
                "F3" => Key.F3,
                "F4" => Key.F4,
                "F5" => Key.F5,
                "F6" => Key.F6,
                "F7" => Key.F7,
                "F8" => Key.F8,
                "F9" => Key.F9,
                "F10" => Key.F10,
                "F11" => Key.F11,
                "F12" => Key.F12,
                "T" => Key.T, // 일반적인 채팅 단축키
                "Y" => Key.Y,
                _ => Key.F6   // 기본값
            };
        }

        /// <summary>
        /// UI 노드를 생성하고 게임 장면에 추가합니다.
        /// </summary>
        private void SetupUi()
        {
            _chatUI = new ChatUI();
            _chatUI.OnMessageSubmitted += OnMessageSubmitted;
            _chatDisplay = new ChatDisplay(_config);
            GD.Print("[MapChat] UI 노드 객체 생성 완료 (아직 씬에 추가되지 않음)");
        }

        /// <summary>
        /// 키보드 입력을 처리하는 핸들러를 등록합니다.
        /// </summary>
        private void RegisterInputHandler()
        {
            // BaseLib의 InputManager를 사용해 입력 이벤트 등록
            // (BaseLib API에 따라 달라질 수 있습니다)
            
            // 방법 1: BaseLib InputManager 사용 (권장)
            // InputManager.RegisterKeyAction(_config.Hotkey, ToggleChat);
            
            // 방법 2: Godot의 기본 입력 시스템 사용
            // _Process에서 Input.IsKeyPressed() 사용
            
            GD.Print($"[MapChat] 입력 핸들러 등록: {_config.Hotkey}");
        }

        /// <summary>
        /// 채팅창 열기/닫기 토글
        /// </summary>
        public void ToggleChat()
        {
            if (_chatUI == null) return;
            
            if (_chatUI.Visible)
                _chatUI.CloseChat();
            else
                _chatUI.OpenChat();
        }

        /// <summary>
        /// 플레이어가 메시지를 입력했을 때 호출됩니다.
        /// </summary>
        private void OnMessageSubmitted(string text)
        {
            GD.Print($"[MapChat] 메시지 수신: {text}");
            
            // 자신의 화면에 표시
            _chatDisplay?.AddMessage(text, GetPlayerName());
            
            // TODO: 멀티플레이어 동기화
            // BaseLib의 네트워킹 API를 사용해 다른 플레이어에게 전송
            // NetworkManager.BroadcastMessage("mapchat_message", text);
        }

        /// <summary>
        /// 현재 플레이어의 이름을 가져옵니다.
        /// </summary>
        private string GetPlayerName()
        {
            // TODO: 실제 플레이어 이름 가져오기
            // BaseLib API를 통해 가져올 수 있을 것입니다
            return "플레이어";
        }

        /// <summary>
        /// 매 프레임 호출 - 단축키 감지
        /// </summary>
        public void Update()
        {
            // 채팅창이 열려 있는 동안은 단축키 무시
            if (_chatUI?.Visible == true) return;
            
            // 단축키 감지
            if (Input.IsKeyPressed(_hotkeyCode))
            {
                ToggleChat();
            }
        }
    }
}