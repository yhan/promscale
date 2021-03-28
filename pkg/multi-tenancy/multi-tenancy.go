package multi_tenancy

import (
	"fmt"

	"github.com/timescale/promscale/pkg/multi-tenancy/config"
	"github.com/timescale/promscale/pkg/multi-tenancy/read"
	"github.com/timescale/promscale/pkg/multi-tenancy/write"
)

var ErrUnauthorized = fmt.Errorf("unauthorized token or tenant")

type MultiTenancy interface {
	// ReadAuthorizer returns a authorizer that authorizes read operations.
	ReadAuthorizer() read.Authorizer
	// WriteAuthorizer returns a authorizer that authorizes write operations.
	WriteAuthorizer() write.Authorizer
}

// multiTenancy type implements the multi-tenancy concept in Promscale.
type multiTenancy struct {
	write write.Authorizer
	read  read.Authorizer
}

// NewMultiTenancy returns a new MultiTenancy type.
func NewMultiTenancy(c *config.Config) (MultiTenancy, error) {
	var (
		readAuthr  read.Authorizer
		writeAuthr write.Authorizer
		err        error
	)
	switch c.AuthType {
	case config.Allow:
		readAuthr, err = read.NewPlainReadAuthorizer(c)
		if err != nil {
			return nil, fmt.Errorf("creating multi-tenancy: %w", err)
		}
		writeAuthr = write.NewPlainWriteAuthorizer(c)
	case config.BearerToken:
		if c.BearerToken == "" {
			return nil, fmt.Errorf("'bearer_token' type of multi-tenancy should have a non-empty bearer_token")
		}
		readAuthr, err = read.NewBearerTokenReadAuthorizer(c)
		if err != nil {
			return nil, fmt.Errorf("creating multi-tenancy: %w", err)
		}
		writeAuthr = write.NewBearerTokenWriteAuthorizer(c)
	default:
		return nil, fmt.Errorf("invalid multi-tenancy type: %d", c.AuthType)
	}
	return &multiTenancy{
		read:  readAuthr,
		write: writeAuthr,
	}, nil
}

func (mt *multiTenancy) ReadAuthorizer() read.Authorizer {
	return mt.read
}

func (mt *multiTenancy) WriteAuthorizer() write.Authorizer {
	return mt.write
}

type noopMultiTenancy struct{}

// NewNoopMultiTenancy returns a No-op multi-tenancy that is used to initialize multi-tenancy types for no operations.
func NewNoopMultiTenancy() MultiTenancy {
	return &noopMultiTenancy{}
}

func (np *noopMultiTenancy) ReadAuthorizer() read.Authorizer {
	return nil
}

func (np *noopMultiTenancy) WriteAuthorizer() write.Authorizer {
	return nil
}
