Imports System.Configuration
Imports System.Data
Imports System.Data.OleDb

Public Class LoginForm
    Dim username As String
    Dim password As String
    Dim connString As String = ConfigurationManager.ConnectionStrings("db").ConnectionString

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

        Close()
    End Sub

    Private Function GetAdminAccount() As DataTable
        Dim userinfo As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_Username], [_Password] FROM tblAccount WHERE [_AccountID] = 1", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                userinfo.Load(reader)
                If userinfo.Rows.Count > 0 Then
                    username = userinfo.Rows(0).Item("_Username").ToString()
                    password = userinfo.Rows(0).Item("_Password").ToString()
                End If
            End Using
        End Using
        Return userinfo
    End Function

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        GetAdminAccount()
        If txtUser.Text = username And txtPass.Text = password Then
            MessageBox.Show("WELCOME ADMINISTRATOR", "LOGIN SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Dim mf As New MainForm
            mf.Show()
            Me.Hide()
            txtUser.Text = ""
            txtPass.Text = ""
        Else
            MessageBox.Show("USER CREDENTIALS INVALID TRY AGAIN!", "LOGIN FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

End Class