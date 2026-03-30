using Godot;
using System;

namespace MapChat
{
    /// <summary>
    /// 인게임 채팅 입력 UI입니다.
    /// F6을 누르면 화면 하단에 텍스트 입력창이 나타납니다.
    /// </summary>
    public partial class ChatUI : Control
    {
        // ==========================================
        // 이벤트: 텍스트가 제출되었을 때 발생
        // MapChatMod.cs에서 이 이벤트를 구독합니다
        // ==========================================
        public event Action<string>? OnMessageSubmitted;

        // UI 구성 요소들
        private Panel? _backgroundPanel;   // 입력창 배경
        private LineEdit? _textInput;      // 텍스트 입력 필드
        private Button? _sendButton;       // 전송 버튼
        private Label? _hintLabel;         // 힌트 텍스트 ("Enter: 전송, ESC: 취소")

        // UI가 현재 표시 중인지 여부
        public bool IsVisible => Visible;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            // UI 숨김 상태로 시작
            Visible = false;
            
            // UI 구성
            SetupUI();
            
            GD.Print("[MapChat] ChatUI 초기화 완료");
        }

        /// <summary>
        /// UI 요소들을 생성하고 배치합니다.
        /// </summary>
        private void SetupUI()
        {
            // 화면 전체 크기에 맞추기
            SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            
            // ---- 배경 패널 ----
            _backgroundPanel = new Panel();
            _backgroundPanel.SetAnchorsAndOffsetsPreset(LayoutPreset.BottomWide);
            _backgroundPanel.Size = new Vector2(0, 60); // 높이 60px
            
            // 패널 스타일 설정 (반투명 검정 배경)
            var styleBox = new StyleBoxFlat();
            styleBox.BgColor = new Color(0, 0, 0, 0.8f);
            styleBox.BorderWidthTop = 2;
            styleBox.BorderColor = new Color(1, 1, 1, 0.5f);
            _backgroundPanel.AddThemeStyleboxOverride("panel", styleBox);
            
            AddChild(_backgroundPanel);
            
            // ---- 레이아웃 컨테이너 ----
            var hbox = new HBoxContainer();
            hbox.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            hbox.AddThemeConstantOverride("separation", 8);
            hbox.OffsetLeft = 10;
            hbox.OffsetRight = -10;
            _backgroundPanel.AddChild(hbox);
            
            // ---- 힌트 레이블 ----
            _hintLabel = new Label();
            _hintLabel.Text = "채팅:";
            _hintLabel.AddThemeColorOverride("font_color", new Color(1, 1, 0)); // 노란색
            _hintLabel.VerticalAlignment = VerticalAlignment.Center;
            _hintLabel.CustomMinimumSize = new Vector2(50, 0);
            hbox.AddChild(_hintLabel);
            
            // ---- 텍스트 입력 필드 ----
            _textInput = new LineEdit();
            _textInput.SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.Fill;
            _textInput.PlaceholderText = "메시지를 입력하세요... (Enter: 전송, ESC: 취소)";
            _textInput.MaxLength = 100; // 최대 100자
            _textInput.AddThemeColorOverride("font_color", new Color(1, 1, 1));
            _textInput.AddThemeFontSizeOverride("font_size", 20);
            
            // Enter 키 입력 시 전송
            _textInput.TextSubmitted += OnTextSubmitted;
            
            hbox.AddChild(_textInput);
            
            // ---- 전송 버튼 ----
            _sendButton = new Button();
            _sendButton.Text = "전송";
            _sendButton.CustomMinimumSize = new Vector2(60, 0);
            _sendButton.Pressed += () => SubmitCurrentText();
            hbox.AddChild(_sendButton);
        }

        /// <summary>
        /// 채팅 입력창을 엽니다.
        /// </summary>
        public void OpenChat()
        {
            Visible = true;
            _textInput?.Clear();
            _textInput?.GrabFocus(); // 입력 포커스 이동
            
            GD.Print("[MapChat] 채팅창 열림");
        }

        /// <summary>
        /// 채팅 입력창을 닫습니다.
        /// </summary>
        public void CloseChat()
        {
            Visible = false;
            _textInput?.ReleaseFocus();
            
            GD.Print("[MapChat] 채팅창 닫힘");
        }

        /// <summary>
        /// 현재 입력된 텍스트를 전송합니다.
        /// </summary>
        private void SubmitCurrentText()
        {
            string text = _textInput?.Text?.Trim() ?? "";
            
            if (!string.IsNullOrEmpty(text))
            {
                OnTextSubmitted(text);
            }
            else
            {
                // 빈 텍스트면 그냥 닫기
                CloseChat();
            }
        }

        /// <summary>
        /// 텍스트가 제출되었을 때 처리합니다.
        /// </summary>
        private void OnTextSubmitted(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                CloseChat();
                return;
            }

            GD.Print($"[MapChat] 메시지 전송: {text}");
            
            // 이벤트 발생 (MapChatMod.cs에서 처리)
            OnMessageSubmitted?.Invoke(text);
            
            CloseChat();
        }

        // 매 프레임마다 호출됩니다
        public override void _Input(InputEvent @event)
        {
            // UI가 표시 중일 때만 처리
            if (!Visible) return;
            
            // ESC 키로 닫기
            if (@event is InputEventKey keyEvent && keyEvent.Pressed)
            {
                if (keyEvent.Keycode == Key.Escape)
                {
                    CloseChat();
                    GetViewport().SetInputAsHandled(); // 이벤트 소비 (다른 곳에 전달 안 함)
                }
            }
        }
    }
}