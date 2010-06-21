﻿using System;
using Machine.Specifications;
using Relax.Impl;
using Relax.Impl.Http;
using Symbiote.Core.Extensions;

namespace Relax.Tests.Repository
{
    public abstract class with_get_document_by_key_and_rev_command : with_document_repository
    {
        protected static Guid id;

        private Establish context = () =>
                                        {
                                            id = Guid.NewGuid();
                                            uri = new CouchUri("http", "localhost", 5984, "relax")
                                                .IdAndRev(id, "2");
                                            commandMock
                                                .Setup(x => x.Get(couchUri))
                                                .Returns("{{ _id : \"{0}\", _rev : \"2\", Message : \"Hello\"}}".AsFormat(id));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}