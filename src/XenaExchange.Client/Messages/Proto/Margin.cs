// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: margin.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Api {

  /// <summary>Holder for reflection information generated from margin.proto</summary>
  public static partial class MarginReflection {

    #region Descriptor
    /// <summary>File descriptor for margin.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MarginReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxtYXJnaW4ucHJvdG8SA2FwaSJ5CgxNYXJnaW5BbW91bnQSHQoJTWFyZ2lu",
            "QW10GO0MIAEoCVIJbWFyZ2luQW10EiUKDU1hcmdpbkFtdFR5cGUY7AwgASgJ",
            "Ug1tYXJnaW5BbXRUeXBlEiMKDE1hcmdpbkFtdENjeRjuDCABKAlSDG1hcmdp",
            "bkFtdENjeSKrAgoXTWFyZ2luUmVxdWlyZW1lbnRSZXBvcnQSGAoHTXNnVHlw",
            "ZRgjIAEoCVIHbXNnVHlwZRI4ChZBY2NvdW50U3RhdHVzUmVxdWVzdElkGJHM",
            "AiABKAlSFmFjY291bnRTdGF0dXNSZXF1ZXN0SWQSLwoSTWFyZ2luUmVxbXRS",
            "cHRUeXBlGMcOIAEoCVISbWFyZ2luUmVxbXRScHRUeXBlEhgKB0FjY291bnQY",
            "ASABKARSB2FjY291bnQSOAoNTWFyZ2luQW1vdW50cxjrDCADKAsyES5hcGku",
            "TWFyZ2luQW1vdW50Ug1tYXJnaW5BbW91bnRzEiMKDFJlamVjdFJlYXNvbhj8",
            "AiABKAlSDHJlamVjdFJlYXNvbhISCgRUZXh0GDogASgJUgR0ZXh0YgZwcm90",
            "bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Api.MarginAmount), global::Api.MarginAmount.Parser, new[]{ "MarginAmt", "MarginAmtType", "MarginAmtCcy" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Api.MarginRequirementReport), global::Api.MarginRequirementReport.Parser, new[]{ "MsgType", "AccountStatusRequestId", "MarginReqmtRptType", "Account", "MarginAmounts", "RejectReason", "Text" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class MarginAmount : pb::IMessage<MarginAmount> {
    private static readonly pb::MessageParser<MarginAmount> _parser = new pb::MessageParser<MarginAmount>(() => new MarginAmount());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MarginAmount> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Api.MarginReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MarginAmount() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MarginAmount(MarginAmount other) : this() {
      marginAmt_ = other.marginAmt_;
      marginAmtType_ = other.marginAmtType_;
      marginAmtCcy_ = other.marginAmtCcy_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MarginAmount Clone() {
      return new MarginAmount(this);
    }

    /// <summary>Field number for the "MarginAmt" field.</summary>
    public const int MarginAmtFieldNumber = 1645;
    private string marginAmt_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MarginAmt {
      get { return marginAmt_; }
      set {
        marginAmt_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "MarginAmtType" field.</summary>
    public const int MarginAmtTypeFieldNumber = 1644;
    private string marginAmtType_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MarginAmtType {
      get { return marginAmtType_; }
      set {
        marginAmtType_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "MarginAmtCcy" field.</summary>
    public const int MarginAmtCcyFieldNumber = 1646;
    private string marginAmtCcy_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MarginAmtCcy {
      get { return marginAmtCcy_; }
      set {
        marginAmtCcy_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MarginAmount);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MarginAmount other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MarginAmt != other.MarginAmt) return false;
      if (MarginAmtType != other.MarginAmtType) return false;
      if (MarginAmtCcy != other.MarginAmtCcy) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (MarginAmt.Length != 0) hash ^= MarginAmt.GetHashCode();
      if (MarginAmtType.Length != 0) hash ^= MarginAmtType.GetHashCode();
      if (MarginAmtCcy.Length != 0) hash ^= MarginAmtCcy.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (MarginAmtType.Length != 0) {
        output.WriteRawTag(226, 102);
        output.WriteString(MarginAmtType);
      }
      if (MarginAmt.Length != 0) {
        output.WriteRawTag(234, 102);
        output.WriteString(MarginAmt);
      }
      if (MarginAmtCcy.Length != 0) {
        output.WriteRawTag(242, 102);
        output.WriteString(MarginAmtCcy);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (MarginAmt.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(MarginAmt);
      }
      if (MarginAmtType.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(MarginAmtType);
      }
      if (MarginAmtCcy.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(MarginAmtCcy);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MarginAmount other) {
      if (other == null) {
        return;
      }
      if (other.MarginAmt.Length != 0) {
        MarginAmt = other.MarginAmt;
      }
      if (other.MarginAmtType.Length != 0) {
        MarginAmtType = other.MarginAmtType;
      }
      if (other.MarginAmtCcy.Length != 0) {
        MarginAmtCcy = other.MarginAmtCcy;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 13154: {
            MarginAmtType = input.ReadString();
            break;
          }
          case 13162: {
            MarginAmt = input.ReadString();
            break;
          }
          case 13170: {
            MarginAmtCcy = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class MarginRequirementReport : pb::IMessage<MarginRequirementReport> {
    private static readonly pb::MessageParser<MarginRequirementReport> _parser = new pb::MessageParser<MarginRequirementReport>(() => new MarginRequirementReport());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MarginRequirementReport> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Api.MarginReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MarginRequirementReport() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MarginRequirementReport(MarginRequirementReport other) : this() {
      msgType_ = other.msgType_;
      accountStatusRequestId_ = other.accountStatusRequestId_;
      marginReqmtRptType_ = other.marginReqmtRptType_;
      account_ = other.account_;
      marginAmounts_ = other.marginAmounts_.Clone();
      rejectReason_ = other.rejectReason_;
      text_ = other.text_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MarginRequirementReport Clone() {
      return new MarginRequirementReport(this);
    }

    /// <summary>Field number for the "MsgType" field.</summary>
    public const int MsgTypeFieldNumber = 35;
    private string msgType_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MsgType {
      get { return msgType_; }
      set {
        msgType_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "AccountStatusRequestId" field.</summary>
    public const int AccountStatusRequestIdFieldNumber = 42513;
    private string accountStatusRequestId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string AccountStatusRequestId {
      get { return accountStatusRequestId_; }
      set {
        accountStatusRequestId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "MarginReqmtRptType" field.</summary>
    public const int MarginReqmtRptTypeFieldNumber = 1863;
    private string marginReqmtRptType_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MarginReqmtRptType {
      get { return marginReqmtRptType_; }
      set {
        marginReqmtRptType_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Account" field.</summary>
    public const int AccountFieldNumber = 1;
    private ulong account_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Account {
      get { return account_; }
      set {
        account_ = value;
      }
    }

    /// <summary>Field number for the "MarginAmounts" field.</summary>
    public const int MarginAmountsFieldNumber = 1643;
    private static readonly pb::FieldCodec<global::Api.MarginAmount> _repeated_marginAmounts_codec
        = pb::FieldCodec.ForMessage(13146, global::Api.MarginAmount.Parser);
    private readonly pbc::RepeatedField<global::Api.MarginAmount> marginAmounts_ = new pbc::RepeatedField<global::Api.MarginAmount>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Api.MarginAmount> MarginAmounts {
      get { return marginAmounts_; }
    }

    /// <summary>Field number for the "RejectReason" field.</summary>
    public const int RejectReasonFieldNumber = 380;
    private string rejectReason_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string RejectReason {
      get { return rejectReason_; }
      set {
        rejectReason_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Text" field.</summary>
    public const int TextFieldNumber = 58;
    private string text_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Text {
      get { return text_; }
      set {
        text_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MarginRequirementReport);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MarginRequirementReport other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MsgType != other.MsgType) return false;
      if (AccountStatusRequestId != other.AccountStatusRequestId) return false;
      if (MarginReqmtRptType != other.MarginReqmtRptType) return false;
      if (Account != other.Account) return false;
      if(!marginAmounts_.Equals(other.marginAmounts_)) return false;
      if (RejectReason != other.RejectReason) return false;
      if (Text != other.Text) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (MsgType.Length != 0) hash ^= MsgType.GetHashCode();
      if (AccountStatusRequestId.Length != 0) hash ^= AccountStatusRequestId.GetHashCode();
      if (MarginReqmtRptType.Length != 0) hash ^= MarginReqmtRptType.GetHashCode();
      if (Account != 0UL) hash ^= Account.GetHashCode();
      hash ^= marginAmounts_.GetHashCode();
      if (RejectReason.Length != 0) hash ^= RejectReason.GetHashCode();
      if (Text.Length != 0) hash ^= Text.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Account != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Account);
      }
      if (MsgType.Length != 0) {
        output.WriteRawTag(154, 2);
        output.WriteString(MsgType);
      }
      if (Text.Length != 0) {
        output.WriteRawTag(210, 3);
        output.WriteString(Text);
      }
      if (RejectReason.Length != 0) {
        output.WriteRawTag(226, 23);
        output.WriteString(RejectReason);
      }
      marginAmounts_.WriteTo(output, _repeated_marginAmounts_codec);
      if (MarginReqmtRptType.Length != 0) {
        output.WriteRawTag(186, 116);
        output.WriteString(MarginReqmtRptType);
      }
      if (AccountStatusRequestId.Length != 0) {
        output.WriteRawTag(138, 225, 20);
        output.WriteString(AccountStatusRequestId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (MsgType.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(MsgType);
      }
      if (AccountStatusRequestId.Length != 0) {
        size += 3 + pb::CodedOutputStream.ComputeStringSize(AccountStatusRequestId);
      }
      if (MarginReqmtRptType.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(MarginReqmtRptType);
      }
      if (Account != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Account);
      }
      size += marginAmounts_.CalculateSize(_repeated_marginAmounts_codec);
      if (RejectReason.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(RejectReason);
      }
      if (Text.Length != 0) {
        size += 2 + pb::CodedOutputStream.ComputeStringSize(Text);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MarginRequirementReport other) {
      if (other == null) {
        return;
      }
      if (other.MsgType.Length != 0) {
        MsgType = other.MsgType;
      }
      if (other.AccountStatusRequestId.Length != 0) {
        AccountStatusRequestId = other.AccountStatusRequestId;
      }
      if (other.MarginReqmtRptType.Length != 0) {
        MarginReqmtRptType = other.MarginReqmtRptType;
      }
      if (other.Account != 0UL) {
        Account = other.Account;
      }
      marginAmounts_.Add(other.marginAmounts_);
      if (other.RejectReason.Length != 0) {
        RejectReason = other.RejectReason;
      }
      if (other.Text.Length != 0) {
        Text = other.Text;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Account = input.ReadUInt64();
            break;
          }
          case 282: {
            MsgType = input.ReadString();
            break;
          }
          case 466: {
            Text = input.ReadString();
            break;
          }
          case 3042: {
            RejectReason = input.ReadString();
            break;
          }
          case 13146: {
            marginAmounts_.AddEntriesFrom(input, _repeated_marginAmounts_codec);
            break;
          }
          case 14906: {
            MarginReqmtRptType = input.ReadString();
            break;
          }
          case 340106: {
            AccountStatusRequestId = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
