// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: Protos.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Proto.Cluster.SingleRemoteInstance {

  /// <summary>Holder for reflection information generated from Protos.proto</summary>
  public static partial class ProtosReflection {

    #region Descriptor
    /// <summary>File descriptor for Protos.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ProtosReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxQcm90b3MucHJvdG8SFHNpbmdsZXJlbW90ZWluc3RhbmNlIgoKCEdldEtp",
            "bmRzIiEKEEdldEtpbmRzUmVzcG9uc2USDQoFS2luZHMYASADKAlCJaoCIlBy",
            "b3RvLkNsdXN0ZXIuU2luZ2xlUmVtb3RlSW5zdGFuY2ViBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Cluster.SingleRemoteInstance.GetKinds), global::Proto.Cluster.SingleRemoteInstance.GetKinds.Parser, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Proto.Cluster.SingleRemoteInstance.GetKindsResponse), global::Proto.Cluster.SingleRemoteInstance.GetKindsResponse.Parser, new[]{ "Kinds" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class GetKinds : pb::IMessage<GetKinds> {
    private static readonly pb::MessageParser<GetKinds> _parser = new pb::MessageParser<GetKinds>(() => new GetKinds());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<GetKinds> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.Cluster.SingleRemoteInstance.ProtosReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetKinds() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetKinds(GetKinds other) : this() {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetKinds Clone() {
      return new GetKinds(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as GetKinds);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(GetKinds other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(GetKinds other) {
      if (other == null) {
        return;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  public sealed partial class GetKindsResponse : pb::IMessage<GetKindsResponse> {
    private static readonly pb::MessageParser<GetKindsResponse> _parser = new pb::MessageParser<GetKindsResponse>(() => new GetKindsResponse());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<GetKindsResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Proto.Cluster.SingleRemoteInstance.ProtosReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetKindsResponse() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetKindsResponse(GetKindsResponse other) : this() {
      kinds_ = other.kinds_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetKindsResponse Clone() {
      return new GetKindsResponse(this);
    }

    /// <summary>Field number for the "Kinds" field.</summary>
    public const int KindsFieldNumber = 1;
    private static readonly pb::FieldCodec<string> _repeated_kinds_codec
        = pb::FieldCodec.ForString(10);
    private readonly pbc::RepeatedField<string> kinds_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Kinds {
      get { return kinds_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as GetKindsResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(GetKindsResponse other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!kinds_.Equals(other.kinds_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= kinds_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      kinds_.WriteTo(output, _repeated_kinds_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += kinds_.CalculateSize(_repeated_kinds_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(GetKindsResponse other) {
      if (other == null) {
        return;
      }
      kinds_.Add(other.kinds_);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            kinds_.AddEntriesFrom(input, _repeated_kinds_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
