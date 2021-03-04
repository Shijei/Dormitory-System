Imports System.Configuration
Imports System.Data
Imports System.Data.OleDb

Public Class MainForm

    Public Property Stringpass As String
    Dim connString As String = ConfigurationManager.ConnectionStrings("db").ConnectionString
    Dim userEndDate As Date
    Dim struserEndDate As String
    Dim slidenum As Integer = 0
    Dim today As Date = Date.Today
    Dim day As String = today.Day
    Dim month As String = today.Month
    Dim year As String = today.Year
    Dim showdate As String
    Dim textMonth As String
    Dim currentDate As Date = Date.Now()
    Dim endDate As String = currentDate.ToString("MM\/dd\/yyyy")
    Dim tempRoomID As String
    Dim flag As Integer
    Dim billbalance As Double
    Dim occupantname As String
    Dim renewDate As String
    Dim contactnum As String
    Dim roomid As String
    Dim roomtype As String
    Dim searchquery As String
    Dim months3 As Double
    Dim months6 As Double
    Dim months12 As Double
    Dim username As String
    Dim password As String
    Dim newpass As String
    Dim confirmpass As String
    Dim updateUserBillBalance As Double
    Dim divisor As Integer
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GetAdminAccount()
        getRates()
        getOccupiedRooms()
        cmbOccupiedRooms.SelectedIndex = 0
        CheckOutDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        ReportDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        cmbSortBox.SelectedIndex = 0
        cmbroomtype.SelectedIndex = 0
        cmbreportsort.SelectedIndex = 0
        CountDouble()
        CountQuadruple()
        CountHexatruple()
        If month = "1" Then
            textMonth = "JANUARY"
        ElseIf month = "2" Then
            textMonth = "FEBRUARY"
        ElseIf month = "3" Then
            textMonth = "MARCH"
        ElseIf month = "4" Then
            textMonth = "APRIL"
        ElseIf month = "5" Then
            textMonth = "MAY"
        ElseIf month = "6" Then
            textMonth = "JUNE"
        ElseIf month = "7" Then
            textMonth = "JULY"
        ElseIf month = "8" Then
            textMonth = "AUGUST"
        ElseIf month = "9" Then
            textMonth = "SEPTEMBER"
        ElseIf month = "10" Then
            textMonth = "OCTOBER"
        ElseIf month = "11" Then
            textMonth = "NOVEMBER"
        ElseIf month = "12" Then
            textMonth = "DECEMBER"
        End If
        showdate = textMonth & " " & day & ", " & year
        lblShowDate.Text = showdate
        Timer1.Start()
    End Sub

    Private Function GetLeavingOccupant() As DataTable
        currentDate = Date.Now()
        endDate = currentDate.ToString("MM\/dd\/yyyy")
        Dim selectLeaving As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_ContactNum] As [User Contact Number],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_EndDate] As [End Date],[_BillBalance] AS [Bill Balance] FROM tblOccupant WHERE [_EndDate] = #" & endDate & "#", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                selectLeaving.Load(reader)
            End Using
        End Using
        Return selectLeaving
    End Function

    Private Function GetOccupantList() As DataTable
        Dim selectAll As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_ContactNum] As [User Contact Number],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_EndDate] As [End Date],[_BillBalance] AS [Bill Balance] FROM tblOccupant", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                selectAll.Load(reader)
            End Using
        End Using
        Return selectAll
    End Function

    Private Function GetOccupantWithBal() As DataTable
        Dim selectBal As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_ContactNum] As [User Contact Number],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_EndDate] As [End Date],[_BillBalance] AS [Bill Balance] FROM tblOccupant WHERE [_BillBalance] <> 0", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                selectBal.Load(reader)
            End Using
        End Using
        Return selectBal
    End Function

    Private Function updateRooms() As DataTable
        Dim updaterm As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblRoom SET [_NoOfOccupants] = [_NoOfOccupants] - 1 WHERE [_RoomID] = '" & roomid & "'", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()
            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return updaterm
    End Function

    Private Function updateSlots() As DataTable
        Dim updateslotrm As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblRoom SET [_AvailableSlots] =  [_MaximumOccupants] - [_NoOfOccupants] WHERE [_RoomID] = '" & roomid & "'", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()
            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return updateslotrm
    End Function

    Private Function updateRates() As DataTable
        Dim updateR As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblRoom SET [_3Months] = " & Val(txt3.Text) & ", [_6Months] = " & Val(txt6.Text) & ", [_12Months] = " & Val(txt12.Text) & " WHERE [_RoomType] = '" & cmbroomtype.Text & "'", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()
                MessageBox.Show("ROOM RATES ARE UPDATED SUCCESSFULLY!", "UPDATE SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return updateR
    End Function

    Private Function getRates() As DataTable
        Dim priceData As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT DISTINCT [_3Months] As [3 Months],[_6Months] As [6 Months], [_12Months] As [12 Months] FROM tblRoom WHERE [_RoomType] ='" & cmbroomtype.Text & "'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                priceData.Load(reader)
                If priceData.Rows.Count > 0 Then
                    months3 = priceData.Rows(0).Item("3 Months").ToString()
                    months6 = priceData.Rows(0).Item("6 Months").ToString()
                    months12 = priceData.Rows(0).Item("12 Months").ToString()
                End If
            End Using
        End Using
        Return priceData
    End Function

    Private Function SortAtoZ() As DataTable
        Dim sortA As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant ORDER BY [_LastName] ASC", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortA.Load(reader)
            End Using
        End Using
        Return sortA
    End Function

    Private Function SortZtoA() As DataTable
        Dim sortZ As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant ORDER BY [_LastName] DESC", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortZ.Load(reader)
            End Using
        End Using
        Return sortZ
    End Function

    Private Function SortMale() As DataTable
        Dim sortM As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant WHERE [_Gender] = 'Male'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortM.Load(reader)
            End Using
        End Using
        Return sortM
    End Function

    Private Function SortFemale() As DataTable
        Dim sortF As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant WHERE [_Gender] = 'Female'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortF.Load(reader)
            End Using
        End Using
        Return sortF
    End Function

    Private Function SortDouble() As DataTable
        Dim sortD As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant WHERE [_RoomType] = 'Double-Sharing'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortD.Load(reader)
            End Using
        End Using
        Return sortD
    End Function

    Private Function SortQuadruple() As DataTable
        Dim sortQ As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant WHERE [_RoomType] = 'Quadruple-Sharing'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortQ.Load(reader)
            End Using
        End Using
        Return sortQ
    End Function

    Private Function SortHexatruple() As DataTable
        Dim sortH As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant WHERE [_RoomType] = 'Hexatruple-Sharing'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortH.Load(reader)
            End Using
        End Using
        Return sortH
    End Function

    Private Function SortWithBalance() As DataTable
        Dim sortWB As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_Gender] As [Gender],[_ContactNum] As [User Contact Number], [_Email] As [Email],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_StartDate] As [Start Date], [_EndDate] As [End Date] FROM tblOccupant WHERE [_BillBalance] <> 0", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                sortWB.Load(reader)
            End Using
        End Using
        Return sortWB
    End Function

    Private Function updatePass() As DataTable
        Dim updatepa As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblAccount SET [_Password] = '" & confirmpass & "'  WHERE [_AccountID] = 1", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()
                MessageBox.Show("ADMIN PASSWORD UPDATED SUCCESSFULLY!", "UPDATE SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return updatepa
    End Function

    Private Function updateRoomBill() As DataTable
        Dim updateRB As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblRoom SET [_ElectricityBill] = " & Val(txtElectricityBill.Text) & "  WHERE [_RoomID] = '" & cmbOccupiedRooms.Text & "'", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()
                MessageBox.Show("THE FEE HAS BEEN APPLIED TO THIS ROOM!", "FEE UPDATE SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return updateRB
    End Function

    Private Function GetDivisor() As DataTable
        Dim diviInfo As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_MaximumOccupants] FROM tblRoom WHERE [_RoomID] = '" & cmbOccupiedRooms.Text & "'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                diviInfo.Load(reader)
                If diviInfo.Rows.Count > 0 Then
                    divisor = diviInfo.Rows(0).Item("_MaximumOccupants").ToString()
                End If
            End Using
        End Using
        Return diviInfo
    End Function

    Private Function updateOccupantBalance() As DataTable
        Dim updateOB As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblOccupant SET [_BillBalance] = " & Val(txtElectricityBill.Text) & "/" & divisor & " WHERE [_RoomID] = '" & cmbOccupiedRooms.Text & "'", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()

            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return updateOB
    End Function

    Private Function payBill() As DataTable
        Dim payB As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblOccupant SET [_BillBalance] = 0 WHERE [_ContactNum] = '" & contactnum & "'", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()
            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return payB
    End Function
  
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

    Private Function getOccupiedRooms() As DataTable
        Dim occupiedRooms As New DataTable
        Dim i As Integer
        Dim rowCount As Integer
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_RoomID] FROM tblRoom WHERE [_NoOfOccupants] <> 0", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                occupiedRooms.Load(reader)
                rowCount = occupiedRooms.Rows.Count
                Do While (i < rowCount)
                    cmbOccupiedRooms.Items.Add(occupiedRooms.Rows(i).Item("_RoomID").ToString)
                    i = i + 1
                Loop
            End Using
        End Using
        Return occupiedRooms
    End Function

    Private Function SearchSomething() As DataTable
        Dim searchContactOrLname As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_LastName] & ', ' & [_FirstName] & ' ' &  [_MiddleName] As [Occupant Name],[_ContactNum] As [User Contact Number],[_RoomID] As [Room ID], [_RoomType] AS [Room Type], [_EndDate] As [End Date],[_BillBalance] AS [Bill Balance] FROM tblOccupant WHERE [_LastName] LIKE '%" & searchquery & "%' OR [_ContactNum] LIKE '%" & searchquery & "%'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                searchContactOrLname.Load(reader)
            End Using
        End Using
        Return searchContactOrLname
    End Function

    Private Function CheckoutAndDeleteOccupant() As DataTable
        Dim deleteOccupant As New DataTable
        Using conn As New OleDbConnection(connString)
            Dim cmd As OleDbCommand = New OleDbCommand("DELETE FROM tblOccupant WHERE [_RoomID] = '" & roomid & "' AND [_ContactNum] = '" & contactnum & "'", conn)
            Try
                conn.Open()
                cmd.ExecuteNonQuery()
                MessageBox.Show("CHECK-OUT SUCCESS, ROOM SLOTS ARE NOW UPDATED!", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As OleDbException
                MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
        Return deleteOccupant
    End Function

    Private Function CountDouble() As DataTable
        Dim countD As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT SUM([_AvailableSlots]) As SumOutput FROM tblRoom WHERE [_RoomType] = 'Double-Sharing' ", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                countD.Load(reader)
                If countD.Rows.Count > 0 Then
                    lblCountD.Text = Val(countD.Rows(0).Item("SumOutput").ToString())
                End If
            End Using
        End Using
        Return countD
    End Function

    Private Function CountQuadruple() As DataTable
        Dim countQ As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT SUM([_AvailableSlots]) As SumOutput FROM tblRoom WHERE [_RoomType] = 'Quadruple-Sharing' ", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                countQ.Load(reader)
                If countQ.Rows.Count > 0 Then
                    lblCountQ.Text = Val(countQ.Rows(0).Item("SumOutput").ToString())
                End If
            End Using
        End Using
        Return countQ
    End Function

    Private Function CountHexatruple() As DataTable
        Dim countQ As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT SUM([_AvailableSlots]) As SumOutput FROM tblRoom WHERE [_RoomType] = 'Hexatruple-Sharing' ", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                countQ.Load(reader)
                If countQ.Rows.Count > 0 Then
                    lblCountH.Text = Val(countQ.Rows(0).Item("SumOutput").ToString())
                End If
            End Using
        End Using
        Return countQ
    End Function

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        slidenum = slidenum + 1
        If slidenum = 1 Then
            lblRoomPhotos.BackgroundImage = My.Resources._4BED
            lblRoomtype.Text = "Quadruple-Sharing"
            btnPrevious.Visible = True
        ElseIf slidenum = 2 Then
            lblRoomPhotos.BackgroundImage = My.Resources._6BED
            lblRoomtype.Text = "Hexatruple-Sharing"
            btnPrevious.Visible = True
            btnNext.Visible = False
        End If
    End Sub

    Private Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        slidenum = slidenum - 1
        If slidenum = 0 Then
            lblRoomPhotos.BackgroundImage = My.Resources._2BED
            lblRoomtype.Text = "Double-Sharing"
            btnNext.Visible = True
            btnPrevious.Visible = False
        ElseIf slidenum = 1 Then
            lblRoomPhotos.BackgroundImage = My.Resources._4BED
            lblRoomtype.Text = "Quadruple-Sharing"
            btnNext.Visible = True
        End If
    End Sub

    Private Sub btnToApplication_Click(sender As Object, e As EventArgs) Handles btnToApplication.Click
        ViewPanel.Visible = False
        HomePanel.Visible = True
        MonitoringPanel.Visible = False
        BillingsPanel.Visible = False
        SettingsPanel.Visible = False
        Dim af As New ApplicationForm
        af.Stringpass = lblRoomtype.Text
        af.Show()
    End Sub

    Private Sub btnDashboard_MouseEnter(sender As Object, e As EventArgs) Handles btnDashboard.MouseEnter
        btnDashboard.BackgroundImage = My.Resources.btnDashboardHover
    End Sub
    Private Sub btnDashboard_MouseLeave(sender As Object, e As EventArgs) Handles btnDashboard.MouseLeave
        btnDashboard.BackgroundImage = My.Resources.btnDashboard
    End Sub
    Private Sub btnViewRoom_MouseEnter(sender As Object, e As EventArgs) Handles btnViewRoom.MouseEnter
        btnViewRoom.BackgroundImage = My.Resources.btnViewRoomsHover
    End Sub
    Private Sub btnViewRoom_MouseLeave(sender As Object, e As EventArgs) Handles btnViewRoom.MouseLeave
        btnViewRoom.BackgroundImage = My.Resources.btnViewRooms
    End Sub
    Private Sub btnMonitoring_MouseEnter(sender As Object, e As EventArgs) Handles btnMonitoring.MouseEnter
        btnMonitoring.BackgroundImage = My.Resources.btnMonitoringHover
    End Sub
    Private Sub btnMonitoring_MouseLeave(sender As Object, e As EventArgs) Handles btnMonitoring.MouseLeave
        btnMonitoring.BackgroundImage = My.Resources.btnMonitoring
    End Sub
    Private Sub btnBilling_MouseEnter(sender As Object, e As EventArgs) Handles btnBilling.MouseEnter
        btnBilling.BackgroundImage = My.Resources.btnReportHover1
    End Sub
    Private Sub btnBilling_MouseLeave(sender As Object, e As EventArgs) Handles btnBilling.MouseLeave
        btnBilling.BackgroundImage = My.Resources.btnReport
    End Sub
    Private Sub btnSettings_MouseEnter(sender As Object, e As EventArgs) Handles btnSettings.MouseEnter
        btnSettings.BackgroundImage = My.Resources.btnAccountSettingsHover2
    End Sub
    Private Sub btnSettings_MouseLeave(sender As Object, e As EventArgs) Handles btnSettings.MouseLeave
        btnSettings.BackgroundImage = My.Resources.btnAccountSettings1
    End Sub
    Private Sub btnAbout_MouseEnter(sender As Object, e As EventArgs) Handles btnAbout.MouseEnter
        btnAbout.BackgroundImage = My.Resources.btnAboutHover
    End Sub
    Private Sub btnAbout_MouseLeave(sender As Object, e As EventArgs) Handles btnAbout.MouseLeave
        btnAbout.BackgroundImage = My.Resources.btnAbout
    End Sub
    Private Sub btnLogout_MouseEnter(sender As Object, e As EventArgs) Handles btnLogout.MouseEnter
        btnLogout.BackgroundImage = My.Resources.btnLogoutHover
    End Sub
    Private Sub btnLogout_MouseLeave(sender As Object, e As EventArgs) Handles btnLogout.MouseLeave
        btnLogout.BackgroundImage = My.Resources.btnLogout
    End Sub

    Private Sub btnDashboard_Click(sender As Object, e As EventArgs) Handles btnDashboard.Click
        CountDouble()
        CountQuadruple()
        CountHexatruple()
        ViewPanel.Visible = False
        HomePanel.Visible = True
        MonitoringPanel.Visible = False
        BillingsPanel.Visible = False
        SettingsPanel.Visible = False
    End Sub

    Private Sub btnViewRoom_Click(sender As Object, e As EventArgs) Handles btnViewRoom.Click
        ViewPanel.Visible = True
        HomePanel.Visible = False
        MonitoringPanel.Visible = False
        BillingsPanel.Visible = False
        SettingsPanel.Visible = False
    End Sub

    Private Sub btnMonitoring_Click(sender As Object, e As EventArgs) Handles btnMonitoring.Click
        ViewPanel.Visible = False
        HomePanel.Visible = False
        MonitoringPanel.Visible = True
        BillingsPanel.Visible = False
        SettingsPanel.Visible = False
        CheckOutDataGridView.DataSource = GetLeavingOccupant()
        CheckOutDataGridView.ClearSelection()
        cmbSortBox.SelectedIndex = 0
        contactnum = ""
        roomid = ""
    End Sub

    Private Sub btnBilling_Click(sender As Object, e As EventArgs) Handles btnBilling.Click
        ViewPanel.Visible = False
        HomePanel.Visible = False
        MonitoringPanel.Visible = False
        BillingsPanel.Visible = True
        SettingsPanel.Visible = False
        cmbreportsort.SelectedIndex = 0
        ReportDataGridView.DataSource = SortAtoZ()
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click
        ViewPanel.Visible = False
        HomePanel.Visible = False
        MonitoringPanel.Visible = False
        BillingsPanel.Visible = False
        SettingsPanel.Visible = True
        cmbroomtype.SelectedIndex = 0
        cmbOccupiedRooms.Items.Clear()
        getOccupiedRooms()
        cmbOccupiedRooms.SelectedIndex = 0
        txtOld.Text = ""
        txtConfirm.Text = ""
        txtNew.Text = ""
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lblShowTime.Text = TimeOfDay.ToString("hh:mm tt")
    End Sub

    Private Sub HomePanel_MouseEnter(sender As Object, e As EventArgs) Handles HomePanel.MouseEnter
        CountDouble()
        CountQuadruple()
        CountHexatruple()
    End Sub

    Private Sub CheckOutDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CheckOutDataGridView.CellClick
        Dim index As Integer
        index = e.RowIndex
        Dim selectedRow As DataGridViewRow
        If index < 0 Then

        Else
            selectedRow = CheckOutDataGridView.Rows(index)
            contactnum = selectedRow.Cells(1).Value.ToString()
            roomid = selectedRow.Cells(2).Value.ToString()
            occupantname = selectedRow.Cells(0).Value.ToString()
            userEndDate = selectedRow.Cells(4).Value.ToString()
            struserEndDate = userEndDate.ToString("MM\/dd\/yyyy")
            renewDate = currentDate.ToString("MM\/dd\/yyyy")
            billbalance = selectedRow.Cells(5).Value.ToString()
        End If

    End Sub

    Private Sub btnCheckout_Click(sender As Object, e As EventArgs) Handles btnCheckout.Click

        If billbalance <> 0 Then
            MessageBox.Show("OCCUPANT STILL HAS A " & Environment.NewLine & "REMAINING BALANCE AMOUNTING OF: PHP " & billbalance & Environment.NewLine &
                            "PLEASE SETTLE HIS/HER BALANCE FIRST BEFORE ENDING THE CONTRACT.", "BALANCE FOUND", MessageBoxButtons.OK, MessageBoxIcon.Error)
            contactnum = ""
            roomid = ""
            CheckOutDataGridView.ClearSelection()
        ElseIf billbalance = 0 Then

            If roomid = "" Or contactnum = "" Then
                MessageBox.Show("PLEASE SELECT A ROW FIRST", "INVALID SELECTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                Dim result As DialogResult = MessageBox.Show("ARE YOU SURE YOU WANT TO CHECK OUT THIS USER?",
                              "CHECK OUT CONFIRMATION",
                              MessageBoxButtons.YesNo)

                If result = DialogResult.Yes Then
                    CheckoutAndDeleteOccupant()
                    updateRooms()
                    updateSlots()
                    CheckOutDataGridView.DataSource = GetLeavingOccupant()
                    CheckOutDataGridView.ClearSelection()
                    cmbSortBox.SelectedIndex = 0
                Else
                    contactnum = ""
                    roomid = ""
                    CheckOutDataGridView.ClearSelection()
                End If
            End If

        End If
    End Sub

    Private Sub btnPayBill_Click(sender As Object, e As EventArgs) Handles btnPayBill.Click
        Dim paymentt As String
        Dim change As Double

        If roomid = "" Or contactnum = "" Then
            MessageBox.Show("PLEASE SELECT A ROW FIRST", "INVALID SELECTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            contactnum = ""
            roomid = ""
            CheckOutDataGridView.ClearSelection()
        ElseIf billbalance = 0 Then
            MessageBox.Show("THIS USER IS ALREADY PAID HIS/HER BILL.", "SETTLED PAYMENT", MessageBoxButtons.OK, MessageBoxIcon.Information)
            contactnum = ""
            roomid = ""
            CheckOutDataGridView.ClearSelection()
        Else
            paymentt = InputBox("ENTER PAYMENT TO SETTLE BALANCE: ", "SETTLEMENT FORM", " ")
            If String.IsNullOrWhiteSpace(paymentt) Then
                MessageBox.Show("INPUT CANNOT BE NULL OR ZERO", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                contactnum = ""
                roomid = ""
                CheckOutDataGridView.ClearSelection()
            Else
                Val(paymentt)
                If paymentt = 0 Then
                    MessageBox.Show("INPUT CANNOT BE NULL OR ZERO", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf paymentt < billbalance Then
                    MessageBox.Show("PAYMENT CAN'T ACCEPTED, WE REQUIRE A FULL PAYMENT FOR THE BILLS", "PAYMENT FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf paymentt >= billbalance Then
                    change = paymentt - billbalance
                    MessageBox.Show("PAYMENT SUCCESSFUL, YOUR CHANGE IS: " & change, "PAYMENT SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    payBill()
                    CheckOutDataGridView.DataSource = GetLeavingOccupant()
                    cmbSortBox.SelectedIndex = 0
                    contactnum = ""
                    roomid = ""
                    CheckOutDataGridView.ClearSelection()
                End If
            End If
        End If
    End Sub

    Private Sub btnSort_Click(sender As Object, e As EventArgs) Handles btnSort.Click
        If cmbSortBox.SelectedIndex = 0 Then
            CheckOutDataGridView.DataSource = GetLeavingOccupant()
            CheckOutDataGridView.ClearSelection()
        ElseIf cmbSortBox.SelectedIndex = 1 Then
            CheckOutDataGridView.DataSource = GetOccupantList()
            CheckOutDataGridView.ClearSelection()
        ElseIf cmbSortBox.SelectedIndex = 2 Then
            CheckOutDataGridView.DataSource = GetOccupantWithBal()
            CheckOutDataGridView.ClearSelection()
        End If
        contactnum = ""
        roomid = ""
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        Dim result As DialogResult = MessageBox.Show("DO YOU WANT TO LOGOUT?",
                             "ADMIN CONFIRMATION",
                             MessageBoxButtons.YesNo)

        If result = DialogResult.Yes Then
            MessageBox.Show("GOODBYE ADMINISTRATOR", "LOGOUT SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
            LoginForm.Visible = True
        Else

        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        searchquery = txtSearchBar.Text
        CheckOutDataGridView.DataSource = SearchSomething()
        cmbSortBox.SelectedIndex = 1
        CheckOutDataGridView.ClearSelection()
    End Sub

    Private Sub btnUpdateRate_Click(sender As Object, e As EventArgs) Handles btnUpdateRate.Click
        If txt3.Text = "" Then
            MessageBox.Show("FIELDS CANNOT BE EMPTY", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf txt6.Text = "" Then
            MessageBox.Show("FIELDS CANNOT BE EMPTY", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf txt12.Text = "" Then
            MessageBox.Show("FIELDS CANNOT BE EMPTY", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Dim result As DialogResult = MessageBox.Show("ARE YOU SURE YOU WANT TO UPDATE THE RATE OF THIS ROOM TYPE?",
                             "RATE UPDATE CONFIRMATION",
                             MessageBoxButtons.YesNo)

            If result = DialogResult.Yes Then
                 updateRates()
            Else

            End If
        End If
    End Sub

    Private Sub cmbroomtype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbroomtype.SelectedIndexChanged
        getRates()
        txt3.Text = months3
        txt6.Text = months6
        txt12.Text = months12
    End Sub

    Private Sub btnUpdateEnergyFee_Click(sender As Object, e As EventArgs) Handles btnUpdateEnergyFee.Click
        If txtElectricityBill.Text = "" Then
            MessageBox.Show("THIS FIELD CAN'T BE EMPTY", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Dim result As DialogResult = MessageBox.Show("ARE YOU SURE YOU WANT TO DISTRIBUTE THIS ENERGY FEE FOR THIS ROOM ID?",
                            "BILL DISTRIBUTION CONFIRMATION",
                            MessageBoxButtons.YesNo)

            If result = DialogResult.Yes Then
                updateRoomBill()
                GetDivisor()
                updateOccupantBalance()
                txtElectricityBill.Text = ""
                cmbOccupiedRooms.SelectedIndex = 0
            Else

            End If           
        End If
    End Sub

    Private Sub btnreportsort_Click(sender As Object, e As EventArgs) Handles btnreportsort.Click
        If cmbreportsort.SelectedIndex = 0 Then
            ReportDataGridView.DataSource = SortAtoZ()
            ReportDataGridView.ClearSelection()
        ElseIf cmbreportsort.SelectedIndex = 1 Then
            ReportDataGridView.DataSource = SortZtoA()
            ReportDataGridView.ClearSelection()
        ElseIf cmbreportsort.SelectedIndex = 2 Then
            ReportDataGridView.DataSource = SortMale()
            ReportDataGridView.ClearSelection()
        ElseIf cmbreportsort.SelectedIndex = 3 Then
            ReportDataGridView.DataSource = SortFemale()
            ReportDataGridView.ClearSelection()
        ElseIf cmbreportsort.SelectedIndex = 4 Then
            ReportDataGridView.DataSource = SortDouble()
            ReportDataGridView.ClearSelection()
        ElseIf cmbreportsort.SelectedIndex = 5 Then
            ReportDataGridView.DataSource = SortQuadruple()
            ReportDataGridView.ClearSelection()
        ElseIf cmbreportsort.SelectedIndex = 6 Then
            ReportDataGridView.DataSource = SortHexatruple()
            ReportDataGridView.ClearSelection()
        ElseIf cmbreportsort.SelectedIndex = 7 Then
            ReportDataGridView.DataSource = SortWithBalance()
            ReportDataGridView.ClearSelection()
        End If
    End Sub

    Private Sub btnUpdatePass_Click(sender As Object, e As EventArgs) Handles btnUpdatePass.Click
        GetAdminAccount()
        If txtOld.Text <> password Then
            MessageBox.Show("WRONG OLD PASSWORD", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf txtNew.Text <> txtConfirm.Text Then
            MessageBox.Show("PASSWORD MISMATCH", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf txtNew.Text = "" Or txtOld.Text = "" Or txtConfirm.Text = "" Then
            MessageBox.Show("PLEASE COMPLETE THE FIELDS", "INVALID INPUT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            confirmpass = txtConfirm.Text
            updatePass()
            txtOld.Text = ""
            txtNew.Text = ""
            txtConfirm.Text = ""
        End If
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click

        'create empty string
        Dim the_html_file As String = String.Empty

        Dim stylesheet As String = "  table.gridtable {margin:0 auto;width:95%;overflow:auto;font-family: helvetica,arial,sans-serif;"
        stylesheet &= "font-size:14px;color:#333333;border-width: 1px;border-color: #666666;border-collapse: collapse;text-align: center}"
        stylesheet &= "table.gridtable th {border-width: 1px;padding: 8px; border-style: solid;border-color: #666666;background-color: #8f866a;}"
        stylesheet &= "table.gridtable td {border-width: 1px;padding: 8px;border-style: solid;border-color: #666666;}"

        the_html_file = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>REPORTS</title><style>" & stylesheet & "</style></head><body>"

        the_html_file &= "<h1 style='font-family:Century Gothic; padding-left: 40px'>LUMIERE RESIDENCES REPORT</h1>"
        the_html_file &= "<table class='gridtable'>"
        the_html_file &= "<thead><tr>"

        'get the column headers
        For Each column As DataGridViewColumn In ReportDataGridView.Columns
            the_html_file = the_html_file & "<th>" & column.HeaderText & "</th>"
        Next

        the_html_file = the_html_file & "</tr></thead><tbody>"

        'get the rows
        For Each row As DataGridViewRow In ReportDataGridView.Rows
            the_html_file &= "<tr>"
            'get the cells
            For Each cell As DataGridViewCell In row.Cells

                Dim cellcontent As String = cell.FormattedValue
                'replace < and > with html entities
                cellcontent = Replace(cellcontent, "<", "&lt;")
                cellcontent = Replace(cellcontent, ">", "&gt;")

                'using inline styles for column 1
                'If cell.ColumnIndex = 1 Then
                '    the_html_file = the_html_file & "<td style=color:white;background-color:red;font-weight:bold>" & cellcontent & "</td>"
                'Else
                '    the_html_file = the_html_file & "<td>" & cellcontent & "</td>"
                'End If

                'no inline styles
                the_html_file = the_html_file & "<td>" & cellcontent & "</td>"

            Next
            the_html_file &= "</tr>"
        Next

        the_html_file &= "</tbody></table></body></html>"

        'write the file
        My.Computer.FileSystem.WriteAllText("C:\Users\Shijei\Documents\Visual Studio 2013\Projects\LumiereResidences\printform.html", the_html_file, False)

        'pass file to default browser
        Dim pinfo As New ProcessStartInfo()
        pinfo.WindowStyle = ProcessWindowStyle.Normal
        pinfo.FileName = "C:\Users\Shijei\Documents\Visual Studio 2013\Projects\LumiereResidences\printform.html"
        Dim p As Process = Process.Start(pinfo)

    End Sub

    Private Sub btnAbout_Click(sender As Object, e As EventArgs) Handles btnAbout.Click
        Dim aff As New AboutForm
        aff.Show()
        ViewPanel.Visible = False
        HomePanel.Visible = True
        MonitoringPanel.Visible = False
        BillingsPanel.Visible = False
        SettingsPanel.Visible = False
    End Sub

    Private Sub btnPrintContract_Click(sender As Object, e As EventArgs) Handles btnPrintContract.Click
        If contactnum = "" Or roomid = "" Then
            MessageBox.Show("PLEASE SELECT A ROW FIRST", "INVALID SELECTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            'create empty string
            Dim the_html_file As String = String.Empty
            Dim stylesheet As String = "  p.paragraphset {align='center'; font-family: Century Gothic; font-size: 18; padding-left: 20px;"


            the_html_file = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>CONTRACT</title><style>" & stylesheet & "</style></head><body>"
            the_html_file &= "<h1 align='center' style='font-family:Century Gothic;'>LUMIERE RESIDENCES DORMITORY AGREEMENT</h1>"
            the_html_file &= "<h4 align='left' style='font-family:Century Gothic; padding-left: 20px;'>OCCUPANT NAME: " & occupantname & "</h4>"
            the_html_file &= "<h4 align='left' style='font-family:Century Gothic; padding-left: 20px;'>OCCUPANT CONTACT #: " & contactnum & "</h4>"
            the_html_file &= "<h4 align='left' style='font-family:Century Gothic; padding-left: 20px;'>END DATE: " & struserEndDate & "</h4>"
            the_html_file &= "<p class = 'paragraphset' >Agreement form<br>WELCOME TO LUMIERE RESIDENCES! As an occupant and housing manager, it is our sincere " &
                            "  desire to provide you with comfortable, clean, safe living and work environment.  Lumiere Residences  " &
                            " Dormitory provides employee dorm rooms as a privilege to its employees. While dorm room size and " &
                            " availability is modest, the Dormitory offers this living accommodation as an additional benefit to help " &
                            " defer the cost and inconvenience of obtaining housing off Lumiere Residences Dormitory property. Lumiere Residences " & "" &
                             " Dormitory owns and controls the dorms and a few off property housing provided for employees. Sun  " & "" &
                            " Valley Dormitory expects all employee-housing residents to treat each other and the physical property   " & "" &
                            " itself respectfully.   " & "<br><br><br><br>" &
                            " 1. <b>TERM</b>: The term of this Dorm Agreement shall begin on the 'check-in date' set forth above, and  " & "" &
                            " shall continue on a day to day basis thereafter on the same terms and conditions unless earlier   " & "" &
                            " terminated as provided for herein.  This Dorm Agreement applies as long as you are living in the    " & "" &
                            " dorms.    " & "<br><br><br>" &
                            " 2. <b>RENT</b>: Rent will be deducted directly from your Lumiere Residences Dormitory paychecks bi-weekly. If you     " & "" &
                            " have any questions about this please ask the Housing Manager. If you find that your rent is not being " & "" &
                            " deducted from your paychecks you must report this to the dorm manager immediately to correct this    " & "" &
                            " problem. You will still be responsible for any accumulated rent that has not been deducted; Sun     " & "" &
                            " Valley Dormitory will deduct amount on the following pay period. It is your responsibility to check      " & "" &
                            " your pay stubs for any deductions being made from Lumiere Residences Dormitory. Rent will be charged on a     " & "" &
                            " (per day) basis bi-weekly and will begin when you receive the key to your dorm room and stopped the     " & "" &
                            " day your key is returned to the dorm office. It is your responsibility to check out and hand in your     " & "" &
                            " dorm key to the dorm office.     " & "<br></p>"
            the_html_file &= "<h4 align='left' style='font-family:Century Gothic; padding-left: 20px;'>OCCUPANT SIGNATURE OVER PRINTED NAME: <u>" & occupantname & "</u> </h4>"
            'write the file
            My.Computer.FileSystem.WriteAllText("C:\Users\Shijei\Documents\Visual Studio 2013\Projects\LumiereResidences\printform.html", the_html_file, False)

            'pass file to default browser
            Dim pinfo As New ProcessStartInfo()
            pinfo.WindowStyle = ProcessWindowStyle.Normal
            pinfo.FileName = "C:\Users\Shijei\Documents\Visual Studio 2013\Projects\LumiereResidences\printform.html"
            Dim p As Process = Process.Start(pinfo)
        End If
    End Sub

End Class

