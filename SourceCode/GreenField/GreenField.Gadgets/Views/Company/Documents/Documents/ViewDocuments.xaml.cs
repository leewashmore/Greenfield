﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Updation data used in controls
    /// </summary>
    public class UpdationData
    {
        /// <summary>
        /// Updation Tag
        /// </summary>
        public UpdationTags UpdationTag { get; set; }

        /// <summary>
        /// Updation Information
        /// </summary>
        public Object UpdationInfo { get; set; }
        
        /// <summary>
        /// Updation Element
        /// </summary>
        public TextBlock UpdationElement { get; set; }
    }

    /// <summary>
    /// Comment Updation Data used in controls
    /// </summary>
    public class CommentUpdationData
    {
        /// <summary>
        /// Comment updation information
        /// </summary>
        public Object CommentUpdationInfo { get; set; }
        
        /// <summary>
        /// Comment updation input textbox
        /// </summary>
        public TextBox CommentUpdationInput { get; set; }

        /// <summary>
        /// Comment insertion grid
        /// </summary>
        public Grid CommentInsertionGrid { get; set; }

        /// <summary>
        /// Comment alert input
        /// </summary>
        public RadSplitButton CommentAlertInput { get; set; }
    }

    /// <summary>
    /// Updation Tags
    /// </summary>
    public enum UpdationTags
    {
        CATEGORY_NAME,
        DOCUMENT_NAME,
        COMPANY_NAME
    }

    /// <summary>
    /// Code behind for ViewDocuments
    /// </summary>
    public partial class ViewDocuments : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Stores updation information
        /// </summary>
        List<UpdationData> updateInfo = new List<UpdationData>();

        /// <summary>
        /// Stores document categorical information received from event invoker
        /// </summary>
        List<DocumentCategoricalData> documentCategoricalInfo = new List<DocumentCategoricalData>(); 
        #endregion

        #region Properties
        /// <summary>
        /// Stores view model reference
        /// </summary>
        private ViewModelDocuments dataContextViewModelDocuments;
        public ViewModelDocuments DataContextViewModelDocuments
        {
            get { return dataContextViewModelDocuments; }
            set { dataContextViewModelDocuments = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextViewModelDocuments != null)
                {
                    DataContextViewModelDocuments.IsActive = isActive;
                }
            }
        } 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelDocuments</param>
        public ViewDocuments(ViewModelDocuments dataContextSource)
        {
            InitializeComponent();
            DataContextViewModelDocuments = dataContextSource;
            this.DataContext = dataContextSource;
            dataContextSource.ConstructDocumentSearchResultEvent += new ConstructDocumentSearchResultEventHandler(ConstructDocumentSearchResult);
        }
        #endregion        

        #region Event Handlers
        /// <summary>
        /// DocumentsTreeView Selected EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RadRoutedEventArgs</param>
        void DocumentsTreeView_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            this.DocumentsTreeView.SelectedItem = null;
        }

        /// <summary>
        /// Update comment on documentation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void DocumentCommentUpdation(object sender, RoutedEventArgs e)
        {
            RadButton element = sender as RadButton;
            if (element != null)
            {
                CommentUpdationData commentUpdationData = element.Tag as CommentUpdationData;
                if (commentUpdationData != null)
                {
                    DocumentCategoricalData data = commentUpdationData.CommentUpdationInfo as DocumentCategoricalData;
                    if (data != null)
                    {
                        DocumentCategoricalData selectedDocument = documentCategoricalInfo.Where(record => record == data).FirstOrDefault();
                        if (selectedDocument != null)
                        {
                            if (DataContextViewModelDocuments.DbInteractivity != null)
                            {
                                DataContextViewModelDocuments.BusyIndicatorNotification(true, "Updating comment to document...");
                                DataContextViewModelDocuments.DbInteractivity.SetDocumentComment(UserSession.SessionManager.SESSION.UserName
                                    , (Int64)selectedDocument.DocumentCatalogData.FileId, commentUpdationData.CommentUpdationInput.Text
                                    , DataContextViewModelDocuments.SetDocumentCommentCallbackMethod);

                                if ((commentUpdationData.CommentAlertInput.DropDownContent as RadListBox).SelectedItems.Count > 0)
                                {
                                    List<String> emailUsers = new List<string>();
                                    foreach (MembershipUserInfo alertUser in 
                                        (commentUpdationData.CommentAlertInput.DropDownContent as RadListBox).SelectedItems)
                                    {
                                        emailUsers.Add(alertUser.Email);
                                    }

                                    String emailTo = String.Join("|", emailUsers.ToArray());
                                    String emailSubject = "Document Comment Updation Alert";
                                    String emailMessageBody = "Document comment updation notification. Please find the details below:\n"
                                        + "Document Name - " + selectedDocument.DocumentCatalogData.FileName + "\n"
                                        + "Company - " + selectedDocument.DocumentCompanyName + "\n"
                                        + "Type - " + EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(selectedDocument.DocumentCategoryType) + "\n"
                                        + "Comment - " + commentUpdationData.CommentUpdationInput.Text + "\n"
                                        + "Comment By - " + UserSession.SessionManager.SESSION.UserName;
                                    DataContextViewModelDocuments.DbInteractivity.SetMessageInfo(emailTo, null, emailSubject, emailMessageBody, null
                                        , UserSession.SessionManager.SESSION.UserName, DataContextViewModelDocuments.SetMessageInfoCallbackMethod_Comment);
                                }
                            }                        
                        }
                    }
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Constructs document search result on user interface
        /// </summary>
        /// <param name="data">DocumentCategoricalData</param>
        private void ConstructDocumentSearchResult(List<DocumentCategoricalData> data)
        {
            if (data == null)
                return;

            documentCategoricalInfo = data;
            updateInfo = new List<UpdationData>();

            List<DocumentCategoryType> distinctDocumentCategoryTypes = data
                .Select(record => record.DocumentCategoryType).ToList().Distinct().ToList();

            this.DocumentsTreeView.Items.Clear();

            foreach (DocumentCategoryType documentCategoryType in distinctDocumentCategoryTypes)
            {
                RadTreeViewItem categoryTreeViewItem = InsertTreeViewItem_Category(documentCategoryType);

                List<DocumentCategoricalData> categoryTypeFilteredData = data.Where(record =>
                    record.DocumentCategoryType == documentCategoryType)
                    .OrderByDescending(record => record.DocumentCatalogData.FileUploadedOn).ToList();

                foreach (DocumentCategoricalData record in categoryTypeFilteredData)
                {
                    switch (record.DocumentCategoryType)
                    {
                        case DocumentCategoryType.COMPANY_MEETING_NOTES:
                        case DocumentCategoryType.COMPANY_ISSUED_DOCUMENTS:
                        case DocumentCategoryType.EARNING_CALLS:
                        case DocumentCategoryType.MODELS:
                        case DocumentCategoryType.IC_PRESENTATIONS:
                        case DocumentCategoryType.BROKER_REPORTS:
                        case DocumentCategoryType.COMPANY_FINANCIAL_FILINGS:
                            categoryTreeViewItem.Items.Add(InsertTreeViewItem_Document(record));
                            break;
                        case DocumentCategoryType.BLOG:
                            categoryTreeViewItem.Items.Add(InsertTreeViewItem_Blog(record));
                            break;
                        default:
                            break;
                    }
                }
                this.DocumentsTreeView.Items.Add(categoryTreeViewItem);
            }
            UpdateNotification();
        }

        /// <summary>
        /// Inserts categories to the root tree to create first level nodes
        /// </summary>
        /// <param name="documentCategoryType">DocumentCategoryType enumeration object</param>
        /// <returns>RadTreeViewItem for specific DocumentCategoryType</returns>
        private RadTreeViewItem InsertTreeViewItem_Category(DocumentCategoryType documentCategoryType)
        {
            RadTreeViewItem item = new RadTreeViewItem();

            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            #region Category Name
            TextBlock headerCategoryName = new TextBlock()
                {
                    Text = EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(documentCategoryType),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold,
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };

            headerCategoryName.SetValue(Grid.ColumnProperty, 0);
            #endregion

            #region Category Updation Notification
            TextBlock headerUpdateNotification = new TextBlock()
                {
                    Text = "*NEW*",
                    Margin = new Thickness(2, 0, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Red),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Visibility = Visibility.Collapsed,
                    FontWeight = FontWeights.Bold,
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
            #endregion

            updateInfo.Add(new UpdationData()
            {
                UpdationTag = UpdationTags.CATEGORY_NAME,
                UpdationInfo = documentCategoryType,
                UpdationElement = headerUpdateNotification
            });

            headerUpdateNotification.SetValue(Grid.ColumnProperty, 1);

            headerGrid.Children.Add(headerCategoryName);
            headerGrid.Children.Add(headerUpdateNotification);

            item.Header = headerGrid;

            return item;
        }

        /// <summary>
        /// Inserts Content in the specific treeview node (category) specific to document type data
        /// </summary>
        /// <param name="data">DocumentCategoricalData</param>
        /// <returns>RadTreeViewItem to be inserted in the category node</returns>
        private RadTreeViewItem InsertTreeViewItem_Document(DocumentCategoricalData data)
        {
            RadTreeViewItem documentTreeViewItem = new RadTreeViewItem() { HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch };

            RadExpander headerExpander = new RadExpander()
            {
                ExpandDirection = Telerik.Windows.Controls.ExpandDirection.Down,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                HorizontalHeaderAlignment = System.Windows.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                FlowDirection = System.Windows.FlowDirection.RightToLeft
            };

            #region Expander Header
            Grid headerExpanderHeaderGrid = new Grid() { FlowDirection = System.Windows.FlowDirection.LeftToRight };

            headerExpanderHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            headerExpanderHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
            headerExpanderHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            headerExpanderHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });

            #region Document Name Grid
            Grid headerExpanderHeaderGridDocumentNameGrid = new Grid() { Margin = new Thickness(-8, 0, 0, 0) };
            headerExpanderHeaderGridDocumentNameGrid.SetValue(Grid.ColumnProperty, 0);
            headerExpanderHeaderGridDocumentNameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerExpanderHeaderGridDocumentNameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            HyperlinkButton fileNameHyperlink = new HyperlinkButton()
            {
                Content = data.DocumentCatalogData.FileName,
                NavigateUri = new Uri(data.DocumentCatalogData.FilePath, UriKind.RelativeOrAbsolute),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black),
                Style = (Style)(this.Resources["HyperlinkButtonStyle"])
            };

            fileNameHyperlink.SetValue(Grid.ColumnProperty, 0);

            TextBlock fileNameUpdateNotification = new TextBlock()
            {
                Text = "*NEW*",
                Margin = new Thickness(2, 0, 0, 0),
                Foreground = new SolidColorBrush(Colors.Red),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Visibility = Visibility.Collapsed,
                Style = (Style)(this.Resources["TextBlockStyle"])
            };

            updateInfo.Add(new UpdationData()
            {
                UpdationTag = UpdationTags.DOCUMENT_NAME,
                UpdationInfo = data,
                UpdationElement = fileNameUpdateNotification
            });

            fileNameUpdateNotification.SetValue(Grid.ColumnProperty, 1);

            headerExpanderHeaderGridDocumentNameGrid.Children.Add(fileNameHyperlink);
            headerExpanderHeaderGridDocumentNameGrid.Children.Add(fileNameUpdateNotification);
            #endregion

            #region Document Company Name
            TextBlock headerExpanderHeaderGridDocumentCompanyName = new TextBlock()
                {
                    Text = data.DocumentCompanyName,// + " (" + data.DocumentSecurityTicker + ")",
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
            headerExpanderHeaderGridDocumentCompanyName.SetValue(Grid.ColumnProperty, 1);
            #endregion

            #region Document Uploaded By
            TextBlock headerExpanderHeaderGridDocumentUploadedBy = new TextBlock()
                {
                    Text = data.DocumentCatalogData.FileUploadedBy,
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
            headerExpanderHeaderGridDocumentUploadedBy.SetValue(Grid.ColumnProperty, 2);
            #endregion

            #region Document Uploaded On
            TextBlock headerExpanderHeaderGridDocumentUploadedOn = new TextBlock()
                {
                    Text = data.DocumentCatalogData.FileUploadedOn.ToLocalTime().ToString("MMMM dd, yyyy"),
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
            headerExpanderHeaderGridDocumentUploadedOn.SetValue(Grid.ColumnProperty, 3);
            #endregion

            headerExpanderHeaderGrid.Children.Add(headerExpanderHeaderGridDocumentNameGrid);
            headerExpanderHeaderGrid.Children.Add(headerExpanderHeaderGridDocumentCompanyName);
            headerExpanderHeaderGrid.Children.Add(headerExpanderHeaderGridDocumentUploadedBy);
            headerExpanderHeaderGrid.Children.Add(headerExpanderHeaderGridDocumentUploadedOn);

            headerExpander.Header = headerExpanderHeaderGrid;
            #endregion

            #region Expander Content
            Border headerExpanderContentBorder = new Border() { BorderThickness = new Thickness(0, 1, 0, 1), BorderBrush = new SolidColorBrush(Colors.Black) };
            Grid headerExpanderContentGrid = new Grid() { FlowDirection = System.Windows.FlowDirection.LeftToRight };
            headerExpanderContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            headerExpanderContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            #region Comment Listing
            Grid headerExpanderContentGridCommentGrid = new Grid() { Margin = new Thickness(0, 5, 0, 0) };
            headerExpanderContentGridCommentGrid.SetValue(Grid.ColumnProperty, 0);
            headerExpanderContentGridCommentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerExpanderContentGridCommentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerExpanderContentGridCommentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            foreach (CommentDetails comment in data.CommentDetails.OrderByDescending(record => record.CommentOn))
            {
                InsertComment_Documents(comment, headerExpanderContentGridCommentGrid);
            }

            headerExpanderContentGrid.Children.Add(headerExpanderContentGridCommentGrid);
            #endregion

            #region Content Updation
            Grid headerExpanderContentGridUpdationGrid = new Grid();
            headerExpanderContentGridUpdationGrid.SetValue(Grid.RowProperty, 1);
            headerExpanderContentGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //headerExpanderContentGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerExpanderContentGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerExpanderContentGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            #region Comment Box
            TextBox documentCommentUpdation = new TextBox()
                {
                    Margin = new Thickness(5),
                    MaxLength = 255,
                    Style = (Style)(this.Resources["TextBoxStyle"])
                };
            documentCommentUpdation.SetValue(Grid.ColumnProperty, 0);
            #endregion

            #region Alert Label
            TextBlock alertLabel = new TextBlock()
                {
                    Text = "Alert:",
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
            alertLabel.SetValue(Grid.ColumnProperty, 1);
            #endregion

            #region Alert User Listing
            RadSplitButton userListingDropDownButton = new RadSplitButton()
            {
                Content = "Alert",
                DropDownHeight = 200,
                Height = (Double)(this.Resources["DefaultControlMinHeight"]),
                Style = (Style)(this.Resources["RadSplitButtonStyle"])
            };

            RadListBox listBox = new RadListBox()
            {
                SelectionMode = Telerik.Windows.Controls.SelectionMode.Multiple,
                ItemsSource = DataContextViewModelDocuments.UserInfo,
                DisplayMemberPath = "UserName",
                Style = (Style)(this.Resources["RadListBoxStyle"])
            };

            userListingDropDownButton.DropDownContent = listBox;
            userListingDropDownButton.SetValue(Grid.ColumnProperty, 1);

            //RadComboBox userListingComboBox = new RadComboBox()
            //{
            //    Margin = new Thickness(5, 0, 0, 0),
            //    Height = (Double)(this.Resources["DefaultControlMinHeight"]),
            //    Style = (Style)(this.Resources["RadComboBoxStyle"])
            //};
            //userListingComboBox.SetValue(Grid.ColumnProperty, 2); 
            #endregion

            CommentUpdationData commentUpdationTagInfo = new CommentUpdationData()
            {
                CommentAlertInput = userListingDropDownButton,
                CommentUpdationInput = documentCommentUpdation,
                CommentInsertionGrid = headerExpanderContentGridCommentGrid,
                CommentUpdationInfo = data
            };

            RadButton commentUpdationButton = new RadButton()
            {
                Margin = new Thickness(5, 0, 5, 0),
                Content = "+",
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                Tag = commentUpdationTagInfo,
                Height = (Double)(this.Resources["DefaultControlMinHeight"]),
                Style = (Style)(this.Resources["RadButtonStyle"])
            };

            commentUpdationButton.SetValue(Grid.ColumnProperty, 2);
            commentUpdationButton.Click += new RoutedEventHandler(DocumentCommentUpdation);

            headerExpanderContentGridUpdationGrid.Children.Add(documentCommentUpdation);
            //headerExpanderContentGridUpdationGrid.Children.Add(alertLabel);
            //headerExpanderContentGridUpdationGrid.Children.Add(userListingComboBox);
            headerExpanderContentGridUpdationGrid.Children.Add(userListingDropDownButton);
            headerExpanderContentGridUpdationGrid.Children.Add(commentUpdationButton);
            #endregion

            headerExpanderContentGrid.Children.Add(headerExpanderContentGridUpdationGrid);
            headerExpanderContentBorder.Child = headerExpanderContentGrid;

            headerExpander.Content = headerExpanderContentBorder;
            #endregion

            documentTreeViewItem.Header = headerExpander;

            return documentTreeViewItem;

        }

        /// <summary>
        /// Inserts Content in the specific treeview node (category) specific to blog type data
        /// </summary>
        /// <param name="data">DocumentCategoricalData</param>
        /// <returns>RadTreeViewItem to be inserted in the category node</returns>
        private RadTreeViewItem InsertTreeViewItem_Blog(DocumentCategoricalData data)
        {
            RadTreeViewItem blogTreeViewItem = new RadTreeViewItem() { HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch };

            #region Header
            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock headerCompanyName = new TextBlock()
            {
                Text = data.DocumentCompanyName,// + " (" + data.DocumentSecurityTicker + ")",
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Style = (Style)(this.Resources["TextBlockStyle"])
            };

            headerCompanyName.SetValue(Grid.ColumnProperty, 0);

            TextBlock headerUpdateNotification = new TextBlock()
            {
                Text = "*NEW*",
                Margin = new Thickness(2, 0, 0, 0),
                Foreground = new SolidColorBrush(Colors.Red),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Visibility = Visibility.Collapsed,
                Style = (Style)(this.Resources["TextBlockStyle"])
            };

            updateInfo.Add(new UpdationData()
            {
                UpdationTag = UpdationTags.COMPANY_NAME,
                UpdationInfo = data,
                UpdationElement = headerUpdateNotification
            });

            headerUpdateNotification.SetValue(Grid.ColumnProperty, 1);

            headerGrid.Children.Add(headerCompanyName);
            headerGrid.Children.Add(headerUpdateNotification);

            blogTreeViewItem.Header = headerGrid;
            #endregion

            #region Sub TreeView
            RadTreeViewItem blogSubTreeViewItem = new RadTreeViewItem() { HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch };

            #region Header
            Border blogSubTreeViewItemHeaderBorder = new Border()
            {
                BorderThickness = new Thickness(0, 1, 0, 1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Margin = new Thickness(-18, 0, 0, 0)
            };

            Grid blogSubTreeViewItemHeaderGrid = new Grid() { FlowDirection = System.Windows.FlowDirection.LeftToRight };
            blogSubTreeViewItemHeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            blogSubTreeViewItemHeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            #region Comment Listing
            Grid blogSubTreeViewItemHeaderGridCommentGrid = new Grid() { Margin = new Thickness(0, 5, 0, 0) };
            blogSubTreeViewItemHeaderGridCommentGrid.SetValue(Grid.ColumnProperty, 0);
            blogSubTreeViewItemHeaderGridCommentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            blogSubTreeViewItemHeaderGridCommentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            blogSubTreeViewItemHeaderGridCommentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            foreach (CommentDetails comment in data.CommentDetails.OrderByDescending(record => record.CommentOn))
            {
                blogSubTreeViewItemHeaderGridCommentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                TextBlock commentOnTextBlock = new TextBlock()
                {
                    Text = comment.CommentOn.ToLocalTime().ToShortDateString(),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
                commentOnTextBlock.SetValue(Grid.RowProperty, blogSubTreeViewItemHeaderGridCommentGrid.RowDefinitions.Count - 1);
                commentOnTextBlock.SetValue(Grid.ColumnProperty, 0);


                TextBlock commentByTextBlock = new TextBlock()
                {
                    Text = comment.CommentBy,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
                commentByTextBlock.SetValue(Grid.RowProperty, blogSubTreeViewItemHeaderGridCommentGrid.RowDefinitions.Count - 1);
                commentByTextBlock.SetValue(Grid.ColumnProperty, 1);

                TextBlock commentTextBlock = new TextBlock()
                {
                    Text = comment.Comment,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
                commentTextBlock.SetValue(Grid.RowProperty, blogSubTreeViewItemHeaderGridCommentGrid.RowDefinitions.Count - 1);
                commentTextBlock.SetValue(Grid.ColumnProperty, 2);

                blogSubTreeViewItemHeaderGridCommentGrid.Children.Add(commentOnTextBlock);
                blogSubTreeViewItemHeaderGridCommentGrid.Children.Add(commentByTextBlock);
                blogSubTreeViewItemHeaderGridCommentGrid.Children.Add(commentTextBlock);
            }

            blogSubTreeViewItemHeaderGrid.Children.Add(blogSubTreeViewItemHeaderGridCommentGrid);
            #endregion

            #region Content Updation
            Grid blogSubTreeViewItemHeaderGridUpdationGrid = new Grid();
            blogSubTreeViewItemHeaderGridUpdationGrid.SetValue(Grid.RowProperty, 1);
            blogSubTreeViewItemHeaderGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            blogSubTreeViewItemHeaderGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            blogSubTreeViewItemHeaderGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            blogSubTreeViewItemHeaderGridUpdationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            TextBox documentCommentUpdation = new TextBox()
            {
                Margin = new Thickness(5),
                Style = (Style)(this.Resources["TextBoxStyle"])
            };
            documentCommentUpdation.SetValue(Grid.ColumnProperty, 0);

            TextBlock alertLabel = new TextBlock()
            {
                Text = "Alert:",
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
                Style = (Style)(this.Resources["TextBlockStyle"])
            };
            alertLabel.SetValue(Grid.ColumnProperty, 1);

            RadComboBox userListingComboBox = new RadComboBox()
            {
                Margin = new Thickness(5, 0, 0, 0),
                Style = (Style)(this.Resources["RadComboBoxStyle"])
            };
            userListingComboBox.SetValue(Grid.ColumnProperty, 2);

            RadButton commentUpdationButton = new RadButton()
            {
                Margin = new Thickness(5, 0, 5, 0),
                Content = "+",
                Style = (Style)(this.Resources["RadButtonStyle"])
            };
            commentUpdationButton.SetValue(Grid.ColumnProperty, 3);

            blogSubTreeViewItemHeaderGridUpdationGrid.Children.Add(documentCommentUpdation);
            blogSubTreeViewItemHeaderGridUpdationGrid.Children.Add(alertLabel);
            blogSubTreeViewItemHeaderGridUpdationGrid.Children.Add(userListingComboBox);
            blogSubTreeViewItemHeaderGridUpdationGrid.Children.Add(commentUpdationButton);
            #endregion

            blogSubTreeViewItemHeaderGrid.Children.Add(blogSubTreeViewItemHeaderGridUpdationGrid);
            blogSubTreeViewItemHeaderBorder.Child = blogSubTreeViewItemHeaderGrid;

            blogSubTreeViewItem.Header = blogSubTreeViewItemHeaderBorder;
            #endregion

            blogTreeViewItem.Items.Add(blogSubTreeViewItem);
            #endregion

            return blogTreeViewItem;
        }        

        /// <summary>
        /// Updates data on user interface
        /// </summary>
        private void UpdateNotification()
        {
            foreach (UpdationData item in updateInfo)
            {
                switch (item.UpdationTag)
                {
                    case UpdationTags.CATEGORY_NAME:
                        if (item.UpdationInfo is DocumentCategoryType)
                        {
                            DocumentCategoryType documentCategoryType = (DocumentCategoryType)item.UpdationInfo;
                            Boolean categoryRequiresNotification = false;

                            foreach (DocumentCategoricalData categoricalData in
                                documentCategoricalInfo.Where(record => record.DocumentCategoryType == documentCategoryType))
                            {
                                if (documentCategoryType == DocumentCategoryType.BLOG)
                                {
                                    foreach (CommentDetails comment in categoricalData.CommentDetails)
                                    {
                                        if (comment.CommentOn >= DateTime.Now.AddHours(-72))
                                            categoryRequiresNotification = true;
                                    }
                                }

                                if (categoricalData.DocumentCatalogData != null)
                                {
                                    if (categoricalData.DocumentCatalogData.FileUploadedOn >= DateTime.Now.AddHours(-72))
                                    {
                                        categoryRequiresNotification = true;
                                        break;
                                    }
                                }
                            }

                            item.UpdationElement.Visibility = categoryRequiresNotification ? Visibility.Visible : Visibility.Collapsed;
                        }
                        break;
                    case UpdationTags.DOCUMENT_NAME:
                        if (item.UpdationInfo is DocumentCategoricalData)
                        {
                            DocumentCategoricalData documentCategoricalData = (DocumentCategoricalData)item.UpdationInfo;
                            Boolean documentRequiresNotification = false;

                            if (documentCategoricalData.DocumentCatalogData != null)
                            {
                                if (documentCategoricalData.DocumentCatalogData.FileUploadedOn >= DateTime.Now.AddHours(-72))
                                    documentRequiresNotification = true;
                            }

                            item.UpdationElement.Visibility = documentRequiresNotification ? Visibility.Visible : Visibility.Collapsed;
                        }
                        break;
                    case UpdationTags.COMPANY_NAME:
                        if (item.UpdationInfo is DocumentCategoricalData)
                        {
                            DocumentCategoricalData blogCategoricalData = (DocumentCategoricalData)item.UpdationInfo;
                            Boolean blogRequiresNotification = false;

                            if (blogCategoricalData.CommentDetails != null)
                            {
                                foreach (CommentDetails comment in blogCategoricalData.CommentDetails)
                                {
                                    if (comment.CommentOn >= DateTime.Now.AddHours(-72))
                                        blogRequiresNotification = true;
                                }
                            }

                            item.UpdationElement.Visibility = blogRequiresNotification ? Visibility.Visible : Visibility.Collapsed;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Insert Comment on documents
        /// </summary>
        /// <param name="comment">Comment</param>
        /// <param name="headerExpanderContentGridCommentGrid">Grid</param>
        private void InsertComment_Documents(CommentDetails comment, Grid headerExpanderContentGridCommentGrid)
        {
            if (comment.Comment != null)
            {
                headerExpanderContentGridCommentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                #region Comment On
                TextBlock commentOnTextBlock = new TextBlock()
                {
                    Text = comment.CommentOn.ToLocalTime().ToString("MMMM dd, yyyy"),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
                commentOnTextBlock.SetValue(Grid.RowProperty, headerExpanderContentGridCommentGrid.RowDefinitions.Count - 1);
                commentOnTextBlock.SetValue(Grid.ColumnProperty, 0);
                #endregion

                #region Comment By
                TextBlock commentByTextBlock = new TextBlock()
                {
                    Text = comment.CommentBy,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
                commentByTextBlock.SetValue(Grid.RowProperty, headerExpanderContentGridCommentGrid.RowDefinitions.Count - 1);
                commentByTextBlock.SetValue(Grid.ColumnProperty, 1);
                #endregion

                #region Comment
                TextBlock commentTextBlock = new TextBlock()
                {
                    Text = comment.Comment,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    Style = (Style)(this.Resources["TextBlockStyle"])
                };
                #endregion

                commentTextBlock.SetValue(Grid.RowProperty, headerExpanderContentGridCommentGrid.RowDefinitions.Count - 1);
                commentTextBlock.SetValue(Grid.ColumnProperty, 2);

                headerExpanderContentGridCommentGrid.Children.Add(commentOnTextBlock);
                headerExpanderContentGridCommentGrid.Children.Add(commentByTextBlock);
                headerExpanderContentGridCommentGrid.Children.Add(commentTextBlock);
            }
        }        
        #endregion

    }
}
