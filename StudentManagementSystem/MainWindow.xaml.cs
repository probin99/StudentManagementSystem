﻿using StudentManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Subject subject = new Subject();
        public List<Subject> subjectList = new List<Subject>();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //focus on first textbox on page load
            Keyboard.Focus(txtStudentName);
        }

        
        private void BtnCreateStudent_Click(object sender, RoutedEventArgs e)
        {
            txtPerformanceOutput.Clear();
            StringBuilder outputString = new StringBuilder();
            outputString.AppendLine("Student Name: " + txtStudentName.Text);
            outputString.AppendLine("Year Level: " + txtYearLevel.Text);
            outputString.AppendLine("Student added to the system. You can now load Assessments using the Load Assessment Button");
            txtPerformanceOutput.Text = outputString.ToString();
            btnLoadAssessment.IsEnabled = true;
        }

        private void TxtStudentName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStudentName.Text))
            {
                //Regex Comparision
                if (Regex.IsMatch(txtStudentName.Text, @"^[a-zA-Z]+$"))
                {
                    txtYearLevel.IsEnabled = true;
                }
                else
                {
                    txtPerformanceOutput.Clear();
                    txtPerformanceOutput.Text = "Please enter student name as string";
                }
            }
            else
            {
                txtPerformanceOutput.Clear();
                txtPerformanceOutput.Text = "Please enter student name";
            }
        }

        private void TxtYearLevel_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtYearLevel.Text))
            {
                if ((txtYearLevel.Text == "11") 
                    || (txtYearLevel.Text == "12") 
                    && (int.TryParse(txtYearLevel.Text, out int yearLevelValueCheck)))
                {
                    btnCreateStudent.IsEnabled = true;
                    txtPerformanceOutput.Text = "You can now add student using the create student button.";
                }
                else
                {
                    txtPerformanceOutput.Clear();
                    txtPerformanceOutput.Text = "Enter year level as 11 or 12 only";
                }
            }
            else
            {
                txtPerformanceOutput.Clear();
                txtPerformanceOutput.Text = "Please enter student year level";
            }
        }

        public void BtnLoadAssessment_Click(object sender, RoutedEventArgs e)
        {
            //Excel file data read
            string filePath = @"C:\Users\Probin\source\repos\StudentManagementSystem\StudentManagementSystem\Excel Data File\COIT20256Ass1Data.csv";

            //Linq query to read csv file
            var lines = File.ReadAllLines(filePath).Skip(1).TakeWhile(t => t != null);

            foreach(var item in lines)
            {
                var values = item.Split(',');
                subjectList.Add(new Subject()
                {
                    SubName = values[0],
                    AssessmentId = Convert.ToDecimal(values[1]),
                    Type = values[2],
                    Topic = values[3],
                    Format = values[4],
                    DueDate = values[5],
                    Mark = false
                });
            }

            //combobox for subjects
            cbSubjects.ItemsSource = subjectList.Select(x => x.SubName).Distinct();
        }


        private void CbSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //combobox for Assessments
            var queryFirst = from a in subjectList where(a.SubName.Contains(cbSubjects.SelectedItem.ToString())) select new { a.SubName, a.AssessmentId, a.Type };
            List<string> filterText = new List<string>();
            foreach(var row in queryFirst)
            {
                filterText.Add(row.SubName + "," + row.AssessmentId + "," + row.Type);
            }
            if (filterText.Count() == queryFirst.Count())
            {
                cbAssessments.ItemsSource = filterText;
            }
        }

        private void BtnDisplayAssessment_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStudentName.Text) && !string.IsNullOrEmpty(txtYearLevel.Text))
            {
                if (cbSubjects.SelectedItem != null)
                {
                    //show relevant data in textbox
                    var querySecond = subjectList.Select(a => a).Where(a => a.SubName.Contains(cbSubjects.SelectedItem.ToString()));
                    string resultA = string.Empty;
                    string resultB = string.Empty;

                    foreach (var p in querySecond)
                    {
                        //Display Assessment Details
                        resultA = p.SubName + ": " + "\n";
                        resultB += "Id: " + p.AssessmentId + ", Type: " + p.Type + ", Topic: " + p.Topic + ", Format: " + p.Format + ", DueDate: " + p.DueDate + ", Achievement: " + p.Mark + "\n";
                    }
                    txtPerformanceOutput.Clear();
                    txtPerformanceOutput.Text = resultA + resultB.ToString();
                }
                else
                {                    
                    txtPerformanceOutput.Clear();
                    txtPerformanceOutput.Text = "Please click load assesment button and select your subject to display corresponding assessments";
                }
            }
            else
            {
                txtPerformanceOutput.Clear();
                txtPerformanceOutput.Text = "Please enter student name and year level first";
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnSetGrade_Click(object sender, RoutedEventArgs e)
        {
            if (cbAssessments.SelectedItem != null)
            {
                if (cbAchievements.IsEnabled && cbAchievements.Text != "Choose Achievement")
                {
                    //get assessment id from assessmentComboBox string
                    string assessmentComboBoxString = cbAssessments.Text.ToString();
                    var getAssessmentID = Regex.Split(assessmentComboBoxString, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "");
                    double idValue = double.Parse(getAssessmentID.ElementAt(0));

                    //set grade
                    var idAndNameCheck = subjectList.Where(a => a.SubName == cbSubjects.Text && a.AssessmentId.ToString() == idValue.ToString()).Select(a => a.AssessmentId);
                    if (idAndNameCheck.Count() > 0)
                    {
                        foreach (decimal id in idAndNameCheck)
                        {
                            subjectList.Where(a => a.SubName == cbSubjects.Text && a.AssessmentId.ToString() == idValue.ToString()).FirstOrDefault().Mark = true;
                        }
                    }
                    string resultA = "Grade has been set";
                    txtPerformanceOutput.Clear();
                    txtPerformanceOutput.Text = resultA;
                }
                else
                {
                    txtPerformanceOutput.Clear();
                    txtPerformanceOutput.Text = "Please select achievement to set grade";
                }
            }
            else
            {
                txtPerformanceOutput.Clear();
                txtPerformanceOutput.Text = "Please select assessment to set grade";
            }
        }
        private void BtnDisplayGrade_Click(object sender, RoutedEventArgs e)
        {
            txtPerformanceOutput.Clear();
            int comboIndex = cbAssessments.SelectedIndex;
            var query = subjectList.Select(a => a).Where(a => a.SubName.Contains(cbSubjects.SelectedItem.ToString()));
            string resultA = string.Empty;
            int i = 0;

            foreach (var p in query)
            {
                if (i == comboIndex)
                {
                    
                    resultA = p.SubName + "\n" 
                        + "Id" + "\t" + p.AssessmentId + "\n"
                        + "Type" + "\t" + p.Type + "\n"
                        + "Topic" + "\t" + p.Topic + "\n"
                        + "Format" + "\t" + p.Format + "\n"
                        + "DueDate" + "\t" + p.DueDate + "\n"
                        + "Achievement" + "\t" + p.Mark;
                }
                i++;
            }
            txtPerformanceOutput.Clear();
            txtPerformanceOutput.Text = resultA;
        }

        private void BtnClearDisplay_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void CbAssessments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbAchievements.IsEnabled = true;
        }

        
    }
}
