package crc64f0d106695cb1f69e;


public class TextAnnotationSelectionBorderView
	extends android.view.View
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTouchEvent:(Landroid/view/MotionEvent;)Z:GetOnTouchEvent_Landroid_view_MotionEvent_Handler\n" +
			"n_onDraw:(Landroid/graphics/Canvas;)V:GetOnDraw_Landroid_graphics_Canvas_Handler\n" +
			"";
		mono.android.Runtime.register ("Syncfusion.SfPdfViewer.XForms.Droid.TextAnnotationSelectionBorderView, Syncfusion.SfPdfViewer.XForms.Android", TextAnnotationSelectionBorderView.class, __md_methods);
	}


	public TextAnnotationSelectionBorderView (android.content.Context p0)
	{
		super (p0);
		if (getClass () == TextAnnotationSelectionBorderView.class)
			mono.android.TypeManager.Activate ("Syncfusion.SfPdfViewer.XForms.Droid.TextAnnotationSelectionBorderView, Syncfusion.SfPdfViewer.XForms.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public TextAnnotationSelectionBorderView (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == TextAnnotationSelectionBorderView.class)
			mono.android.TypeManager.Activate ("Syncfusion.SfPdfViewer.XForms.Droid.TextAnnotationSelectionBorderView, Syncfusion.SfPdfViewer.XForms.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public TextAnnotationSelectionBorderView (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == TextAnnotationSelectionBorderView.class)
			mono.android.TypeManager.Activate ("Syncfusion.SfPdfViewer.XForms.Droid.TextAnnotationSelectionBorderView, Syncfusion.SfPdfViewer.XForms.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public boolean onTouchEvent (android.view.MotionEvent p0)
	{
		return n_onTouchEvent (p0);
	}

	private native boolean n_onTouchEvent (android.view.MotionEvent p0);


	public void onDraw (android.graphics.Canvas p0)
	{
		n_onDraw (p0);
	}

	private native void n_onDraw (android.graphics.Canvas p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}