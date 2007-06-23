using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.ImageArea;
using CustomGtkWidgets.CommonDialogs;

using MathTextLearner.Assistant.BitmapProcessHelpers;

using MathTextLibrary.BitmapProcesses;
using MathTextLibrary.Utils;

namespace MathTextLearner.Assistant
{
	/// <summary>
	/// Esta clase implementa el panel que permite seleccionar que procesado de
	/// imagen se hará al tratar las imagenes al aprender y reconocer con la
	/// base de datos creada por el asistente.
	/// </summary>
	public class BitmapProcessesStep : PanelAssistantStep
	{
	
#region Controles de Glade
		
		[Glade.WidgetAttribute]
		private Frame bitmapProcessesStepFrame;
		
		[Glade.WidgetAttribute]
		private Button removeProcessBtn;		
		
		[Glade.WidgetAttribute]
		private Button upProcessBtn;
		
		[Glade.WidgetAttribute]
		private Button downProcessBtn;
		
		[Glade.WidgetAttribute]
		private Button editProcessBtn;
		
		[Glade.WidgetAttribute]
		private ScrolledWindow bitmapsProcessSW;
			
		[Glade.WidgetAttribute]
		private HBox previewHB;
		
		[Glade.WidgetAttribute]
		private ToggleButton previewTB;
		
		[Glade.WidgetAttribute]
		private VButtonBox processBtnBox;
		
		[Glade.WidgetAttribute]
		private Frame originFrame;
		
		[Glade.WidgetAttribute]
		private Frame processedFrame;
		
		[Glade.WidgetAttribute]
		private Frame imagesFrame;
		
		[Glade.WidgetAttribute]
		private TreeView imagesTV;
		
#endregion Controles de Glade
		
#region Atributos
		
		private NodeView processesView;
		
		private Dictionary<Type,string> bitmapProcessesTypes;
		
		private ImageArea originIA;
		private ImageArea processedIA;		
		
#endregion Atributos
		
#region Constructor
		
		public BitmapProcessesStep(PanelAssistant assistant, ListStore imagesStore) : base(assistant)
		{
			Glade.XML gxml =
				new Glade.XML(null,"databaseAssistant.glade","bitmapProcessesStepFrame",null);
				
			gxml.Autoconnect(this);			
			
			SetRootWidget(bitmapProcessesStepFrame);
			
			InitializeWidgets(imagesStore);
			
			RetrieveBitmapProcessesTypes();
		}
		
#endregion Constructor
		
#region Metodos públicos
		
		public override bool HasErrors ()
		{
			errors = "";
			
			return errors.Length > 0;
		}
		
#endregion Metodos públicos
		
#region Propiedades
		
		public List<BitmapProcess> Processes
		{
			get
			{
				// Construimos una lista con los procesos a partir de los nodos.
				List<BitmapProcess> res = new List<BitmapProcess>();
				foreach(BitmapProcessNode node in processesView.NodeStore)
				{
					res.Add(node.Process);
				}
				
				return res;
			}
		}
		
		
#endregion Propiedades
		
#region Metodos privados
		
		private void CreateProcessedPreview(Gdk.Pixbuf p)
		{
			// Creamos una matriz a partir del pixbuf, y le 
			// aplicamos los procesados.
			float[,] m = ImageUtils.CreateMatrixFromPixbuf(p);
			
			foreach(BitmapProcessNode node in processesView.NodeStore)
			{
				m = node.Process.Apply(m);
			}
			
			Gdk.Pixbuf processedPixbuf = ImageUtils.CreatePixbufFromMatrix(m);
			
			// Asignamos la imagen a su control.
			processedIA.Image = processedPixbuf;			
		}		
		
		private void InitializeWidgets(ListStore imagesStore)
		{
			NodeStore store = new NodeStore(typeof(BitmapProcessNode));
			processesView =  new NodeView(store);			
			
			processesView.ShowExpanders = false;
			
			processesView.RulesHint = true;
			
			processesView.NodeSelection.Changed += OnProcessesSelectionChanged;
			
			processesView.AppendColumn("Proceso", 
			                           new CellRendererText(),
			                           "text",0);
			
			processesView.AppendColumn("Valores",
			                           new CellRendererText(),
			                           "text",1);		
			
			bitmapsProcessSW.Add(processesView);
			
			originIA = new ImageArea();
			processedIA = new ImageArea();

			originIA.ImageMode = ImageAreaMode.Zoom;
			processedIA.ImageMode = ImageAreaMode.Zoom;
					
			originFrame.Add(originIA);
			processedFrame.Add(processedIA);
			
			imagesTV.Model = imagesStore;
			
			imagesTV.AppendColumn("Imagen",new CellRendererText(),"text",1);
			
			imagesTV.Selection.Changed += OnImagesTVSelectionChanged;
		
			bitmapProcessesStepFrame.Shown += OnBitmapProcessesStepFrameShown;
			
		}
		
		private void OnBitmapProcessesStepFrameShown(object sender, EventArgs a)
		{
			previewHB.Visible = false;
		}
		
		private void OnDownProcessBtnClicked(object sender, EventArgs a)
		{
			// Obtenemos el indice del nodo seleccionado.
			int idx = processesView.Selection.GetSelectedRows()[0].Indices[0];
			
			// Eliminamos el nodo
			TreeNode node = (TreeNode)processesView.NodeSelection.SelectedNode;
			processesView.NodeStore.RemoveNode(node);
			
			// Lo insertamos en su sitio
			
			processesView.NodeStore.AddNode(node, idx+1);
			
			processesView.NodeSelection.SelectNode(node);
			TreePath path = processesView.Selection.GetSelectedRows()[0];
			processesView.ScrollToCell(path,null, true,0f,0);
			
			processesView.ColumnsAutosize();
		}
		
		private void OnAddProcessBtnClicked(object sender, EventArgs a)
		{
			
			ProcessSelectorDialog dlg = 
				new ProcessSelectorDialog(
				                          Assistant.Window,
				                          bitmapProcessesTypes);

			ResponseType res = dlg.Run();
			
			
			if(res == ResponseType.Ok)
			{
				// Hemos seleccionado un proceso.				
				Type t = dlg.SelectedProcess;
				
				BitmapProcessNode node = 
					new BitmapProcessNode(
					                      t,
					                      bitmapProcessesTypes[t]);
				                                               
				processesView.NodeStore.AddNode(node);

				processesView.NodeSelection.SelectNode(node);  
				
				TreePath path = processesView.Selection.GetSelectedRows()[0];
				processesView.ScrollToCell(path,null, true,0f,0);
			}
					                               
			dlg.Destroy();		                                
		}
		
		private void OnEditProcessBtnClicked(object e, EventArgs a)
		{

			BitmapProcessNode node =
				processesView.NodeSelection.SelectedNode as BitmapProcessNode;
			
			if(node != null)
			{
				Type t = node.Process.GetType();
				ResponseType res = 
					ProcessEditorDialog.Show(
					                         Assistant.Window,
					                         node.Process,
					                         bitmapProcessesTypes[t]);
				
				processesView.QueueDraw();
				processesView.ColumnsAutosize();
			}
		}
		
		private void OnImagesTVSelectionChanged(object sender, EventArgs a)
		{
			// Comprobamos que hay filas seleccionadas.
			if(imagesTV.Selection.CountSelectedRows() > 0)
			{
				// Recuperamos la ruta de la imagen, y la cargamos en el
				// en la imagen original.
				TreeIter iter;
				imagesTV.Selection.GetSelected(out iter);
				// Recuperamos la ruta de la imagen.
				string path =(string)(imagesTV.Model.GetValue(iter,2));
				
				Gdk.Pixbuf p = new Gdk.Pixbuf(path);
				
				originIA.Image = p;
				
				CreateProcessedPreview(p);
				
			}
			else
			{
				originIA.Image = null;
				processedIA.Image = null;
			}
		}
		
		private void OnPreviewTBToggled(object sender, EventArgs a)
		{
			// Si tenemos previsualización, la ocultamos, y si no, la mostramos.
			if(previewTB.Active)
			{
				processBtnBox.Sensitive = false;				
				bitmapsProcessSW.Visible = false;
				previewHB.Visible = true;
				
			}
			else
			{
				
				
				previewHB.Visible = false;
				bitmapsProcessSW.Visible = true;
				processBtnBox.Sensitive = true;
				
				imagesTV.Selection.UnselectAll();
			}
		}
		
		private void OnProcessesSelectionChanged(object e, EventArgs a)
		{
			
			int selectedIdx = -1;
			if(processesView.Selection.CountSelectedRows() > 0)
		    {
    		    selectedIdx=
					processesView.Selection.GetSelectedRows()[0].Indices[0];  		
    		   
		    }
			
			int count=0;
			
			foreach(TreeNode n in processesView.NodeStore)
			{
				count++;
			}
			
			
			// Activamos los botones segun la posicion del elemento de la lista
			upProcessBtn.Sensitive = selectedIdx > 0;
			
			removeProcessBtn.Sensitive = selectedIdx >= 0;
			downProcessBtn.Sensitive = 
				selectedIdx >=0
				&& selectedIdx < count -1;
			
			// En el caso de editar, comprobamos que el elemento tenga
			// algo que editar
			
			BitmapProcessNode node =
				(BitmapProcessNode)processesView.NodeSelection.SelectedNode;
			
			editProcessBtn.Sensitive = 
				selectedIdx >= 0 
				&& node.ProcessValues != "";
		}
		
		private void OnRemoveProcessBtnClicked(object e, EventArgs a)
		{
			// Eliminamos el nodo
			TreeNode node = (TreeNode)processesView.NodeSelection.SelectedNode;
			processesView.NodeStore.RemoveNode(node);
			processesView.ColumnsAutosize();
		}
		
		private void OnUpProcessBtnClicked(object e, EventArgs a)
		{			
			// Obtenemos el indice del nodo seleccionado.
			int idx = processesView.Selection.GetSelectedRows()[0].Indices[0];
			
			// Eliminamos el nodo
			TreeNode node = (TreeNode)processesView.NodeSelection.SelectedNode;
			processesView.NodeStore.RemoveNode(node);
			
			// Lo insertamos en su sitio
			
			processesView.NodeStore.AddNode(node, idx-1);
			
			processesView.NodeSelection.SelectNode(node);
			
			TreePath path = processesView.Selection.GetSelectedRows()[0];
			processesView.ScrollToCell(path,null, true,0f,0);
		}
		
		private void RetrieveBitmapProcessesTypes()
		{
			bitmapProcessesTypes =  new Dictionary<System.Type,string>();
			
			Assembly a = Assembly.GetAssembly(typeof(BitmapProcess));
			
			foreach(Type t in a.GetTypes())
			{
				if(t.BaseType ==  typeof(BitmapProcess))
				{
					string desc = RetrieveDescription(t);
					bitmapProcessesTypes.Add(t,desc);
				}
			}
			
			
		}
		
		private string RetrieveDescription(Type t)
		{		
			object[] attributes =
				t.GetCustomAttributes(typeof(BitmapProcessDescription),true);
			
			BitmapProcessDescription bd =
				(BitmapProcessDescription)attributes[0];
			return bd.Description;
		}
		
		
#endregion Metodos privados
		
	}
}
