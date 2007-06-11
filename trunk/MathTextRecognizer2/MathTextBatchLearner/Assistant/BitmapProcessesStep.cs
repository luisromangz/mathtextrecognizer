
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.CommonDialogs;

using MathTextBatchLearner.Assistant.BitmapProcessesHelpers;

using MathTextLibrary.BitmapProcesses;

namespace MathTextBatchLearner.Assistant
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
			
		
		#endregion Controles de Glade
		
		#region Atributos
		
		private NodeView processesView;
		
		private Dictionary<Type,string> bitmapProcessesTypes;
		
		
		
		#endregion Atributos
		
		#region Constructor
		
		public BitmapProcessesStep(PanelAssistant assistant) 
			: base(assistant)
		{
			Glade.XML gxml =
				new Glade.XML(null,"gui.glade","bitmapProcessesStepFrame",null);
				
			gxml.Autoconnect(this);			
			
			SetRootWidget(bitmapProcessesStepFrame);
			
			InitializeWidgets();
			
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
		
		#region Metodos privados
		
		private void InitializeWidgets()
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
	
	
	/// <summary>
	/// Esta clase represnta los nodos de la vista de los procesados.
	/// </summary>
	public class BitmapProcessNode
		: TreeNode
	{
		private string description;
		
		private BitmapProcess process;
		
		
		/// <summary>
		/// El constructor de la clase toma un tipo, y creará una instancia
		/// de una subclase de <c>BitmapProcess</c>.
		/// </summary>
		public BitmapProcessNode(Type t, string description)
		{		
			this.description = description;
			
			process =
				(BitmapProcess)(t.GetConstructor(new Type[]{}).Invoke(null));
			
		}		
		
		[TreeNodeValue(Column=0)]
		public string ProcessDescription
		{
			get
			{
				return description;
			}
		}
		
		[TreeNodeValue(Column=1)]
		public string ProcessValues
		{
			get
			{
				return process.Values;
			}
		}
			
		public BitmapProcess Process
		{
				
			get
			{
				return process;
			}
		
		}
	}
}
