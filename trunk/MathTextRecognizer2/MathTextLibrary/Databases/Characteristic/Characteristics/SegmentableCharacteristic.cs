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
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.TopToBottom, true));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.RightToLeft, true));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.BottomToTop, true));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.LeftToRight, true));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.TopToBottom, false));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.RightToLeft, false));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.BottomToTop, false));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.LeftToRight, false));
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
