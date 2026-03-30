using Godot;
using System.Collections.Generic;

namespace MapChat
{
    /// <summary>
    /// 지도 위에 채팅 메시지를 표시하고 관리합니다.
    /// 일정 시간이 지나면 자동으로 사라집니다.
    /// </summary>
    public partial class ChatDisplay : Node
    {
        // 활성 채팅 메시지 목록
        private readonly List<ChatMessage> _activeMessages = new();
        
        // 설정
        private readonly ChatConfig _config;
        
        // 메시지를 표시할 위치 (화면 좌상단 기준)
        private Vector2 _displayPosition = new Vector2(20, 100);

        public ChatDisplay(ChatConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 새 채팅 메시지를 추가합니다.
        /// </summary>
        /// <param name="text">표시할 텍스트</param>
        /// <param name="senderName">보낸 사람 이름</param>
        public void AddMessage(string text, string senderName = "나")
        {
            // 최대 메시지 수 초과 시 가장 오래된 것 제거
            if (_activeMessages.Count >= _config.MaxMessages)
            {
                var oldest = _activeMessages[0];
                oldest.Sprite?.QueueFree();
                _activeMessages.RemoveAt(0);
            }
            
            // 표시할 전체 텍스트 구성 (이름 포함)
            string displayText = $"[{senderName}]: {text}";
            
            // 텍스트를 텍스처로 변환
            var texture = TextRenderer.RenderTextToTexture(
                displayText,
                new Color(1, 1, 1), // 흰색 텍스트
                _config.FontSize
            );
            
            if (texture == null)
            {
                GD.PrintErr("[MapChat] 텍스처 생성 실패");
                return;
            }
            
            // Sprite2D로 지도에 표시
            var sprite = new Sprite2D();
            sprite.Texture = texture;
            
            // 위치 설정 (이전 메시지 아래에 배치)
            float yOffset = _activeMessages.Count * (_config.FontSize + 10);
            sprite.Position = _displayPosition + new Vector2(0, yOffset);
            
            // 장면에 추가
            // TODO: 게임의 맵 레이어에 추가해야 함
            // Engine.GetMainLoop().Root.AddChild(sprite);
            
            // 메시지 기록
            var message = new ChatMessage
            {
                Text = displayText,
                Sprite = sprite,
                RemainingTime = _config.DisplayDuration,
                CreatedAt = Time.GetTicksMsec()
            };
            
            _activeMessages.Add(message);
            
            GD.Print($"[MapChat] 메시지 표시: {displayText}");
        }

        // 매 프레임 호출 - 만료된 메시지 제거
        public override void _Process(double delta)
        {
            for (int i = _activeMessages.Count - 1; i >= 0; i--)
            {
                _activeMessages[i].RemainingTime -= (float)delta;
                
                if (_activeMessages[i].RemainingTime <= 0)
                {
                    // 페이드 아웃 효과 (점점 투명해지기)
                    var sprite = _activeMessages[i].Sprite;
                    if (sprite != null)
                    {
                        // 마지막 2초 동안 페이드 아웃
                        float alpha = Math.Max(0, _activeMessages[i].RemainingTime / 2.0f + 1.0f);
                        sprite.Modulate = new Color(1, 1, 1, alpha);
                    }
                    
                    // 완전히 만료되면 제거
                    if (_activeMessages[i].RemainingTime < -1.0f)
                    {
                        _activeMessages[i].Sprite?.QueueFree();
                        _activeMessages.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 모든 메시지를 즉시 제거합니다.
        /// </summary>
        public void ClearAllMessages()
        {
            foreach (var msg in _activeMessages)
            {
                msg.Sprite?.QueueFree();
            }
            _activeMessages.Clear();
        }
    }

    /// <summary>
    /// 개별 채팅 메시지 데이터입니다.
    /// </summary>
    public class ChatMessage
    {
        public string Text { get; set; } = "";
        public Sprite2D? Sprite { get; set; }
        public float RemainingTime { get; set; }
        public ulong CreatedAt { get; set; }
    }
}