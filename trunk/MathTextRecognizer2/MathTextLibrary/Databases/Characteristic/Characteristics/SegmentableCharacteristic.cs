// SegmentableCharacteristic.cs created with MonoDevelop
// User: luis at 18:22 05/06/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Projection;
using MathTextLibrary.BitmapSegmenters;


namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	
	
	public class SegmentableCharacteristic : BinaryCharacteristic
	{
		
		private List<BitmapSegmenter> segmenters;
		
		public SegmentableCharacteristic() : base()
		{
			this.priority = 10;
			
			segmenters = new List<BitmapSegmenter>();
			
			segmenters.Add(new AllHolesProjectionSegmenter(ProjectionMode.Horizontal));
			segmenters.Add(new AllHolesProjectionSegmenter(ProjectionMode.Vertical));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.TopToBottom, true, true));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.RightToLeft, true, true));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.BottomToTop, true, true));			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.LeftToRight, true, true));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.TopToBottom, false, true));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.RightToLeft, false, true));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.BottomToTop, false, true));			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.LeftToRight, false, true));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.TopToBottom, true, false));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.RightToLeft, true, false));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.BottomToTop, true, false));			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.LeftToRight, true, false));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.TopToBottom, false, false));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.RightToLeft, false, false));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.BottomToTop, false, false));			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.LeftToRight, false, false));
			
		}
		
		public override bool Apply (FloatBitmap image)
		{
			MathTextBitmap bitmap = new MathTextBitmap(image, new Gdk.Point(0,0));
			foreach (BitmapSegmenter segmenter in segmenters) 
			{
				if(segmenter.Segment(bitmap).Count>1)
				{
					return true;
				}
			}
			
			return false;
		}

	}
}
