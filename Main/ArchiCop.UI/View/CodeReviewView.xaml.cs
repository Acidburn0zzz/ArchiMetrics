﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeReviewView.xaml.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for CodeReviewView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.View
{
	using System.IO;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Input;
	using Clipboard = System.Windows.Clipboard;
	using DataFormats = System.Windows.DataFormats;
	using UserControl = System.Windows.Controls.UserControl;

	/// <summary>
	/// Interaction logic for CodeReviewView.xaml
	/// </summary>
	public partial class CodeReviewView : UserControl
	{
		public CodeReviewView()
		{
			InitializeComponent();
		}

		private async void OnPrintReport(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog();
			if (saveDialog.ShowDialog() == DialogResult.OK)
			{
				CodeReviewGrid.SelectAllCells();
				ApplicationCommands.Copy.Execute(null, CodeReviewGrid);
				CodeReviewGrid.UnselectAllCells();
				var data = (string)Clipboard.GetData(DataFormats.Html);
				Clipboard.Clear();
				var writer = new StreamWriter(saveDialog.FileName);
				await writer.WriteAsync(data);
				writer.Close();
			}
		}
	}
}