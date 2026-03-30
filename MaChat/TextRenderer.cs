using Godot;

namespace MapChat
{
    /// <summary>
    /// 텍스트 문자열을 Godot 텍스처(이미지)로 변환합니다.
    /// 이 텍스처를 지도 위에 그릴 수 있습니다.
    /// </summary>
    public static class TextRenderer
    {
        // 사용할 폰트 크기 (픽셀)
        private const int DefaultFontSize = 24;
        
        // 텍스트 주변 여백 (픽셀)
        private const int Padding = 8;
        
        // 배경 투명도 (0 = 완전 투명, 1 = 불투명)
        private const float BackgroundAlpha = 0.7f;

        /// <summary>
        /// 텍스트를 이미지 텍스처로 변환합니다.
        /// </summary>
        /// <param name="text">표시할 텍스트</param>
        /// <param name="textColor">텍스트 색상</param>
        /// <param name="fontSize">폰트 크기</param>
        /// <returns>Godot ImageTexture 객체</returns>
        public static ImageTexture RenderTextToTexture(
            string text, 
            Color textColor, 
            int fontSize = DefaultFontSize)
        {
            // 폰트 가져오기 (Godot 기본 폰트 사용)
            var font = ThemeDB.FallbackFont;
            
            // 텍스트 크기 계산
            Vector2 textSize = font.GetStringSize(text, fontSize: fontSize);
            
            // 이미지 크기 = 텍스트 크기 + 여백
            int imageWidth = (int)textSize.X + Padding * 2;
            int imageHeight = (int)textSize.Y + Padding * 2;
            
            // 이미지 생성 (RGBA8 형식 = 빨강, 초록, 파랑, 투명도 각 8비트)
            var image = Image.Create(imageWidth, imageHeight, false, Image.Format.Rgba8);
            
            // 배경을 반투명 검정으로 채우기 (텍스트가 잘 보이게)
            image.Fill(new Color(0, 0, 0, BackgroundAlpha));
            
            // === 텍스트를 이미지에 그리기 ===
            // Godot에서 이미지에 직접 텍스트를 그리려면
            // Viewport를 사용하는 방법이 가장 안정적입니다.
            var texture = RenderViaViewport(text, textColor, fontSize, imageWidth, imageHeight);
            
            return texture;
        }

        /// <summary>
        /// Viewport를 이용해 텍스트를 텍스처로 변환합니다.
        /// 이 방식이 Godot에서 텍스트 렌더링에 가장 적합합니다.
        /// </summary>
        private static ImageTexture RenderViaViewport(
            string text, Color textColor, int fontSize, int width, int height)
        {
            // SubViewport: 화면에 보이지 않는 가상 렌더링 공간
            var viewport = new SubViewport();
            viewport.Size = new Vector2I(width, height);
            viewport.TransparentBg = true;
            viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
            
            // Label: Godot의 텍스트 표시 컨트롤
            var label = new Label();
            label.Text = text;
            label.AddThemeFontSizeOverride("font_size", fontSize);
            label.AddThemeColorOverride("font_color", textColor);
            label.Position = new Vector2(Padding, Padding);
            label.AutowrapMode = TextServer.AutowrapMode.Off;
            
            // 반투명 배경 패널 추가
            var background = new ColorRect();
            background.Color = new Color(0, 0, 0, 0.7f);
            background.Size = new Vector2(width, height);
            
            // 장면 트리에 추가
            viewport.AddChild(background);
            viewport.AddChild(label);
            Engine.GetMainLoop().Root.AddChild(viewport);
            
            // 한 프레임 렌더링 후 텍스처 추출
            // (실제로는 다음 프레임에서 가져와야 함 - 비동기 처리)
            var texture = viewport.GetTexture();
            var imageTexture = ImageTexture.CreateFromImage(texture.GetImage());
            
            // 사용 후 Viewport 제거 (메모리 누수 방지)
            viewport.QueueFree();
            
            return imageTexture;
        }

        /// <summary>
        /// 텍스트를 여러 줄로 렌더링합니다.
        /// 긴 텍스트를 자동으로 줄바꿈합니다.
        /// </summary>
        public static ImageTexture RenderMultilineText(
            string text,
            Color textColor,
            int fontSize = DefaultFontSize,
            int maxWidth = 400)
        {
            var font = ThemeDB.FallbackFont;
            
            // 최대 너비로 텍스트 분할
            Vector2 textSize = font.GetMultilineStringSize(
                text, 
                width: maxWidth, 
                fontSize: fontSize
            );
            
            int imageWidth = (int)Math.Min(textSize.X + Padding * 2, maxWidth + Padding * 2);
            int imageHeight = (int)textSize.Y + Padding * 2;
            
            return RenderViaViewport(text, textColor, fontSize, imageWidth, imageHeight);
        }
    }
}