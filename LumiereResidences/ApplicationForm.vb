Imports System.Configuration
Imports System.Data
Imports System.Data.OleDb
Public Class ApplicationForm

    Public Property Stringpass As String
    Dim connString As String = ConfigurationManager.ConnectionStrings("db").ConnectionString
    Dim check As Integer
    Dim currentDate As Date = Date.Now()
    Dim strDate As String = currentDate.ToString("MM\/dd\/yyyy")
    Dim endCurrentDate As Date
    Dim strEndDate As String

    Dim firstname, lastname, middlename, occupantname As String
    Dim gender, contact, email, roomtype, roomid, plan As String
    Dim fee As Double
    Dim payment As Double
    Dim balance As Double
    Dim change As Double

    Dim isRoomsEmpty As Integer
    Private Sub ApplicationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblGetRoomtype.Text = Stringpass
        IfRoomsEmpty()
        If isRoomsEmpty <> 0 Then
            cmbContractPlan.SelectedIndex = 0
            RoomDataGridView.DataSource = GetRoomData()
            GetPriceData()
        Else
            MessageBox.Show("NO ROOMS AVAILABLE ON THIS ROOM TYPE!", "AVAILABILITY UPDATE", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End If
    End Sub

    Private Function GetRoomData() As DataTable
        Dim roomData As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_RoomID] As [Room Number], [_AvailableSlots] As [Available Slots] FROM tblRoom WHERE [_RoomType] = '" & lblGetRoomtype.Text & "' ORDER BY [_AvailableSlots] DESC", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                roomData.Load(reader)
            End Using
        End Using
        Return roomData
    End Function

    Private Function GetPriceData() As DataTable
        Dim priceData As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT DISTINCT [_3Months] As [3 Months],[_6Months] As [6 Months], [_12Months] As [12 Months] FROM tblRoom WHERE [_RoomType] ='" & lblGetRoomtype.Text & "'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                priceData.Load(reader)
                If priceData.Rows.Count > 0 Then
                    lbl3.Text = priceData.Rows(0).Item("3 Months").ToString()
                    lbl6.Text = priceData.Rows(0).Item("6 Months").ToString()
                    lbl12.Text = priceData.Rows(0).Item("12 Months").ToString()
                End If
            End Using
        End Using
        Return priceData
    End Function

    Private Function IfRoomsEmpty() As DataTable
        Dim roomsEmpty As New DataTable
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT SUM([_AvailableSlots]) As AvailableSum FROM tblRoom WHERE [_RoomType] ='" & lblGetRoomtype.Text & "'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                roomsEmpty.Load(reader)
                If roomsEmpty.Rows.Count > 0 Then
                    isRoomsEmpty = Val(roomsEmpty.Rows(0).Item("AvailableSum").ToString())
                End If
            End Using
        End Using
        Return roomsEmpty
    End Function

    Private Function GetAvailableRoom() As DataTable
        Dim availableRoomData As New DataTable
        Dim i As Integer
        Dim rowCount As Integer
        Using conn As New OleDbConnection(connString)
            Using cmd As New OleDbCommand("SELECT [_RoomID] FROM tblRoom WHERE [_AvailableSlots] <> 0 AND [_RoomType] = '" & lblGetRoomtype.Text & "'", conn)
                conn.Open()
                Dim reader As OleDbDataReader = cmd.ExecuteReader()
                availableRoomData.Load(reader)
                rowCount = availableRoomData.Rows.Count
                Do While (i < rowCount)
                    cmbRoomNumber.Items.Add(availableRoomData.Rows(i).Item("_RoomID").ToString)
                    i = i + 1
                Loop
            End Using
        End Using
        Return availableRoomData
    End Function

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        GetAvailableRoom()
        FirstPanel.Visible = False
        SecondPanel.Visible = True
        ThirdPanel.Visible = False
        txtRoomType.Text = lblGetRoomtype.Text
        cmbRoomNumber.SelectedIndex = 0
        cmbGender.SelectedIndex = 0
    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        FirstPanel.Visible = True
        SecondPanel.Visible = False
        ThirdPanel.Visible = False
    End Sub


    Private Sub btnPayment_Click(sender As Object, e As EventArgs) Handles btnPayment.Click
        If String.IsNullOrEmpty(txtLN.Text) Or String.IsNullOrEmpty(txtFN.Text) Or
           String.IsNullOrEmpty(txtMN.Text) Or String.IsNullOrEmpty(txtContactNum.Text) Or
           String.IsNullOrEmpty(txtEmail.Text) Then
            MessageBox.Show("PLEASE COMPLETE THE FIELDS!", "INCOMPLETE FIELDS", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf chkConfirm.Checked = False Then
            MessageBox.Show("MAKE IT SURE THE OCCUPANT ACCEPT TERMS AND CONDITIONS!", "TERMS AND CONDITIONS", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else

            ThirdPanel.Visible = True
            SecondPanel.Visible = False
            FirstPanel.Visible = False
            lastname = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtLN.Text)
            firstname = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtFN.Text)
            middlename = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtMN.Text)
            gender = cmbGender.GetItemText(cmbGender.SelectedItem)
            contact = txtContactNum.Text
            email = txtEmail.Text
            roomtype = txtRoomType.Text
            roomid = cmbRoomNumber.GetItemText(cmbRoomNumber.SelectedItem)
            plan = cmbContractPlan.GetItemText(cmbContractPlan.SelectedItem)

            If plan = "3 Months" Then
                fee = Val(lbl3.Text)
                endCurrentDate = Date.Now.AddMonths(3)
            ElseIf plan = "6 Months" Then
                fee = Val(lbl6.Text)
                endCurrentDate = Date.Now.AddMonths(6)
            ElseIf plan = "12 Months" Then
                fee = Val(lbl12.Text)
                endCurrentDate = Date.Now.AddMonths(12)
            End If
            lblName.Text = lastname & ", " & firstname & " " & middlename
            lblGender.Text = gender
            lblContact.Text = contact
            lblEmail.Text = email
            lblRoomType.Text = roomtype
            lblRoomNumber.Text = roomid
            lblPlan.Text = plan
            lblFee.Text = fee
            strEndDate = endCurrentDate.ToString("MM\/dd\/yyyy")
        End If

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        FirstPanel.Visible = False
        SecondPanel.Visible = True
        ThirdPanel.Visible = False
    End Sub

    Private Sub btnConfirmOccupant_Click(sender As Object, e As EventArgs) Handles btnConfirmOccupant.Click      
        payment = Val(txtFullpayment.Text)
        If payment < fee Then
            MessageBox.Show("YOUR PAYMENT SHOULD BE GREATER OR EQUAL TO OUR FEE!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
            payment = 0
        Else
            payment = Val(txtFullpayment.Text)
        End If

        If payment <> 0 Then
            ' ADD THE OCCUPANT TO DATABASE
            Using conn As New OleDbConnection(connString)
                Dim cmd As OleDbCommand = New OleDbCommand("INSERT INTO tblOccupant([_LastName],[_FirstName],[_MiddleName],[_Gender],[_ContactNum],[_Email],[_RoomType],[_RoomID],[_PaymentType],[_StartDate],[_EndDate],[_BillBalance]) VALUES (?,?,?,?,?,?,?,?,?,?,?,?)", conn)

                cmd.Parameters.Add(New OleDbParameter("_LastName", CType(lastname, String)))
                cmd.Parameters.Add(New OleDbParameter("_FirstName", CType(firstname, String)))
                cmd.Parameters.Add(New OleDbParameter("_MiddleName", CType(middlename, String)))
                cmd.Parameters.Add(New OleDbParameter("_Gender", CType(gender, String)))
                cmd.Parameters.Add(New OleDbParameter("_ContactNum", CType(contact, String)))
                cmd.Parameters.Add(New OleDbParameter("_Email", CType(email, String)))
                cmd.Parameters.Add(New OleDbParameter("_RoomType", CType(roomtype, String)))
                cmd.Parameters.Add(New OleDbParameter("_RoomID", CType(roomid, String)))
                cmd.Parameters.Add(New OleDbParameter("_PaymentType", CType(plan, String)))
                cmd.Parameters.Add(New OleDbParameter("_StartDate", CType(strDate, String)))
                cmd.Parameters.Add(New OleDbParameter("_StartDate", CType(strEndDate, String)))
                cmd.Parameters.Add(New OleDbParameter("_BillBalance", CType("0", String)))

                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                    change = payment - fee
                    MessageBox.Show("PAYMENT SUCCESS, YOUR CHANGE IS: " & change, "TRANSACTION COMPLETE", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MessageBox.Show(lastname & ", " & firstname & " " & middlename & " is now an occupant of Lumiere Residences.", "TRANSACTION COMPLETE", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Catch ex As OleDbException When ex.ErrorCode = -2147467259
                    MessageBox.Show("CONTACT NUMBER ALREADY EXISTS IN DATABASE!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Catch ex As OleDbException
                    MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            End Using
            'END OF ADD

            Using conn As New OleDbConnection(connString)
                Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblRoom SET [_NoOfOccupants] = [_NoOfOccupants] + 1 WHERE [_RoomID] = '" & roomid & "'", conn)
                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                Catch ex As OleDbException
                    MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using

            Using conn As New OleDbConnection(connString)
                Dim cmd As OleDbCommand = New OleDbCommand("UPDATE tblRoom SET [_AvailableSlots] =  [_MaximumOccupants] - [_NoOfOccupants] WHERE [_RoomID] = '" & roomid & "'", conn)
                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                Catch ex As OleDbException
                    MessageBox.Show("AN ERROR OCCURED, PLEASE CHECK THE FOLLOWING DATA ENTERED!", "INVALID TRANSACTION", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
            Me.Close()
        End If
    End Sub

End Class