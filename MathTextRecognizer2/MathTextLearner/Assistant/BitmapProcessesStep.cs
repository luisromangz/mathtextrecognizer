using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.ImageArea;
using CustomGtkWidgets.CommonDialogs;

using MathTextLearner.Config;
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
		private Button makeDefaultBtn;
		
		[Glade.WidgetAttribute]
		private ScrolledWindow bitmapsProcessSW;
			
		[Glade.WidgetAttribute]
		private HBox previewHB;
				
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
		
		public BitmapProcessesStep(PanelAssistant assistant,
		                           ListStore imagesStore) : base(assistant)
		{
			Glade.XML gxml = new Glade.XML(null,
			                               "databaseAssistant.glade",
			                               "bitmapProcessesStepFrame",
			                               null);
				
			gxml.Autoconnect(this);			
			
			SetRootWidget(bitmapProcessesStepFrame);
			
			RetrieveBitmapProcessesTypes();
			
			InitializeWidgets(imagesStore);
			
		}
		
#endregion Constructor
		
#region Metodos públicos
		
		/// <summary>
		/// Indica si hay errores de validación en el paso del asistente.
		/// </summary>
		/// <returns>
		/// «true» si hay errores, «false» en caso contrario.
		/// </returns>
		public override bool HasErrors ()
		{
			errors = "";
			
			return errors.Length > 0;
		}
		
#endregion Metodos públicos
		
#region Propiedades
		
		/// <value>
		/// Permite obtener la lista de procesos que se aplicará a las imagenes
		/// procesadas usando esta base de datos. 
		/// </value>
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
		
		/// <summary>
		/// Establece las vistas de una imagen para su previsualizacion.
		/// </summary>
		/// <param name="p">
		/// La imagen que se quiere previsualizar
		/// </param>
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
		
		/// <summary>
		/// Inicializa los controles del panel del asistente.
		/// </summary>
		/// <param name="imagesStore">
		/// El almacén de imagenes creado en el paso anterior, para que las
		/// imagenes sean seleccionables en la vista de previsualización.
		/// </param>
		private void InitializeWidgets(ListStore imagesStore)
		{
			NodeStore store = new NodeStore(typeof(BitmapProcessNode));
			processesView =  new NodeView(store);			
			
			processesView.ShowExpanders = false;
			
			processesView.RulesHint = true;
			
			processesView.NodeSelection.Changed += OnProcessesSelectionChanged;
			
			processesView.AppendColumn("Algoritmo", 
			                           new CellRendererText(),
			                           "text",0);
			
			processesView.AppendColumn("Parámetros",
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
		
			LoadDefaults();
			
		}
		
		/// <summary>
		/// Carga la seleccion de algoritmos de procesado por defecto.
		/// </summary>
		private void LoadDefaults()
		{
			// Obtenemos los algoritmos por defecto.
			List<BitmapProcess> defaults =
				LearnerConfig.Instance.DefaultProcesses;
			
			foreach(BitmapProcess process in defaults)
			{
				AddProcess(process);
			}
		}
		
		/// <summary>
		/// Maneja el uso del boton de bajar un algoritmo en la lista.
		/// </summary>
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
		
		/// <summary>
		/// Maneja el evento del uso del boton de añadir un nuevo algoritmo
		/// a la lista de procesados de imagenes.
		/// </summary>
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
				
				AddProcess(t);
			}
					                               
			dlg.Destroy();		                                
		}
		
		/// <summary>
		/// Añade un algoritmo a la lista.
		/// </summary>
		/// <param name="t">
		/// El tipo del proceso a añadir.
		/// </param>
		private void AddProcess(Type t)
		{
			BitmapProcessNode node = new BitmapProcessNode(t,
			                                               bitmapProcessesTypes[t]);
				                                               
			processesView.NodeStore.AddNode(node);

			processesView.NodeSelection.SelectNode(node);  
			
			TreePath path = processesView.Selection.GetSelectedRows()[0];
			processesView.ScrollToCell(path,null, true,0f,0);
		}
		
		/// <summary>
		/// Añade un algoritmo de procesado a la lista.
		/// </summary>
		/// <param name="p">
		/// A <see cref="BitmapProcess"/>
		/// </param>
		private void AddProcess(BitmapProcess p)
		{
			BitmapProcessNode node = 
				new BitmapProcessNode(p, bitmapProcessesTypes[p.GetType()]);
				                                               
			processesView.NodeStore.AddNode(node);

			processesView.NodeSelection.SelectNode(node);  
			
			TreePath path = processesView.Selection.GetSelectedRows()[0];
			processesView.ScrollToCell(path,null, true,0f,0);
		}
		
		/// <summary>
		/// Maneja el uso del boton de editar los parametros de un algoritmo.
		/// </summary>
		private void OnEditProcessBtnClicked(object e, EventArgs a)
		{

			BitmapProcessNode node =
				processesView.NodeSelection.SelectedNode as BitmapProcessNode;
			
			if(node != null)
			{
				Type t = node.Process.GetType();
				ProcessEditorDialog.Show(Assistant.Window,
				                         node.Process,
				                         bitmapProcessesTypes[t]);
				
				processesView.QueueDraw();
				processesView.ColumnsAutosize();
			}
		}
		
		/// <summary>
		/// Maneja la seleccion de una imagen para crear su vista de
		/// previsualizacion.
		/// </summary>
		private void OnImagesTVSelectionChanged(object sender, EventArgs a)
		{
			// Comprobamos que hay filas seleccionadas.
			if(imagesTV.Selection.CountSelectedRows() > 0)
			{
				// Recuperamos la ruta de la imagen, y la cargamos en el
				// en la imagen original.
				TreeIter iter;
				imagesTV.Selection.GetSelected(out iter);
				
				UpdatePreview(iter);
			}
			else
			{
				originIA.Image = null;
				processedIA.Image = null;
			}
		}	
		
		/// <summary>
		/// Actualiza la previsualizacion de la imagen con los algoritmos
		/// seleccionados.
		/// </summary>
		/// <param name="image">
		/// El elemento de la lista que representa a la imagen.
		/// </param>
		private void UpdatePreview(TreeIter image)
		{
			// Recuperamos la ruta de la imagen.
			string path =(string)(imagesTV.Model.GetValue(image,2));
			
			Gdk.Pixbuf p = new Gdk.Pixbuf(path);
			
			originIA.Image = p;
			
			CreateProcessedPreview(p);
		}
		
		/// <summary>
		/// Maneja el uso del boton encargado de guardar la seleccion actual de 
		/// procesados como seleccion por defecto.
		/// </summary>
		/// <param name="e">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnMakeDefaultBtnClicked(object e, EventArgs args)
		{
			ResponseType res = ConfirmDialog.Show(this.Assistant.Window,
			                                      "Se va a cambiar la configuración por defecto, ¿desea continuar?");
			
			if(res == ResponseType.Yes)
			{
				// Guardamos la seleccion actual como la por defecto.
				LearnerConfig.Instance.DefaultProcesses = this.Processes;
				LearnerConfig.Instance.Save();
			}
		}
			
		/// <summary>
		/// Maneja el cambio de la seleccion en la lista de algoritmos.
		/// </summary>
		/// <param name="e">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
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
		
		/// <summary>
		/// Maneja el cambio de pestañas.
		/// </summary>
		/// <param name="e">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="SelectPageArgs"/>
		/// </param>
		private void OnProcessesNotebookSwitchPage(object e,
		                                           SwitchPageArgs arg)
		{
			// Si vamos a la página delas previsualizaciones,
			// recargamos la imagen por si hemos cambiado los algoritmos
			// en la otra pagina.
			
			if(arg.PageNum == 1)
			{
				if(imagesTV.Selection.CountSelectedRows()==0)		
				{	
					// Si no hay seleccion seleccionamos el primero
					TreeIter firstIter; 
					imagesTV.Model.GetIterFirst(out firstIter);
					imagesTV.Selection.SelectIter(firstIter);
				}
				else
				{
					// Actualizamos la previsualización de la seleccion 
					// actual.
					TreeIter selected;
					imagesTV.Selection.GetSelected(out selected);
					UpdatePreview(selected);
					
				}
			}
		}
		                                   
		
		/// <summary>
		/// Maneja el uso del boton para elminar algoritmos de la lista.
		/// </summary>
		/// <param name="e">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnRemoveProcessBtnClicked(object e, EventArgs a)
		{
			// Eliminamos el nodo
			TreeNode node = (TreeNode)processesView.NodeSelection.SelectedNode;
			processesView.NodeStore.RemoveNode(node);
			processesView.ColumnsAutosize();
		}
		
		/// <summary>
		/// Maneja el uso del boton para subir procesados en la lista.
		/// </summary>
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
		
		/// <summary>
		/// Recupera los tipos que implementan algoritmos de procesado de
		/// imagenes.
		/// </summary>
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
		
		/// <summary>
		/// Obtiene la descripcion de un algoritmo de procesado de imagenes,
		/// usando reflexion sobre el tipo.
		/// </summary>
		/// <param name="t">
		/// El tipo en el que se implementa el algoritmo.
		/// </param>
		/// <returns>
		/// Una cadena de texto con la descripción.
		/// </returns>
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
